//
// Copyright (c) 2009-2010 Krueger Systems, Inc.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;

namespace SimpleSQL
{
	public class SQLiteException : System.Exception
	{
		public SQLite3.Result Result { get; private set; }

		protected SQLiteException (SQLite3.Result r,string message) : base(message)
		{
			Result = r;
		}

		public static SQLiteException New (SQLite3.Result r, string message)
		{
			return new SQLiteException (r, message);
		}
	}

	/// <summary>
	/// Represents an open connection to a SQLite database.
	/// </summary>
	public class SQLiteConnection : IDisposable
	{
		protected bool _open;
        protected TimeSpan _busyTimeout;
        protected Dictionary<string, TableMapping> _mappings = null;
        protected Dictionary<string, TableMapping> _tables = null;
        protected System.Diagnostics.Stopwatch _sw;
        protected long _elapsedMilliseconds = 0;

		public IntPtr Handle { get; private set; }

		public string DatabasePath { get; private set; }

		public bool TimeExecution { get; set; }

		public bool Trace { get; set; }

		/// <summary>
		/// Constructs a new SQLiteConnection and opens a SQLite database specified by databasePath.
		/// </summary>
		/// <param name="databasePath">
		/// Specifies the path to the database file.
		/// </param>
		public SQLiteConnection (string databasePath)
		{
			DatabasePath = databasePath;
			IntPtr handle;
			var r = SQLite3.Open (DatabasePath, out handle);
			Handle = handle;
			if (r != SQLite3.Result.OK) {
				throw SQLiteException.New (r, "Could not open database file: " + DatabasePath);
			}
			_open = true;
			
			BusyTimeout = TimeSpan.FromSeconds (0.1);
		}

		static SQLiteConnection ()
		{
			if (_preserveDuringLinkMagic) {
				var ti = new TableInfo ();
				ti.name = "magic";
			}
		}

		/// <summary>
		/// Used to list some code that we want the MonoTouch linker
		/// to see, but that we never want to actually execute.
		/// </summary>
		static bool _preserveDuringLinkMagic = false;

		/// <summary>
		/// Sets a busy handler to sleep the specified amount of time when a table is locked.
		/// The handler will sleep multiple times until a total time of <see cref="BusyTimeout"/> has accumulated.
		/// </summary>
		public TimeSpan BusyTimeout {
			get { return _busyTimeout; }
			set {
				_busyTimeout = value;
				if (Handle != IntPtr.Zero) {
					SQLite3.BusyTimeout (Handle, (int)_busyTimeout.TotalMilliseconds);
				}
			}
		}

		/// <summary>
		/// Returns the mappings from types to tables that the connection
		/// currently understands.
		/// </summary>
		public IEnumerable<TableMapping> TableMappings {
			get {
				if (_tables == null) {
					return Enumerable.Empty<TableMapping> ();
				} else {
					return _tables.Values;
				}
			}
		}

		/// <summary>
		/// Retrieves the mapping that is automatically generated for the given type.
		/// </summary>
		/// <param name="type">
		/// The type whose mapping to the database is returned.
		/// </param>
		/// <returns>
		/// The mapping represents the schema of the columns of the database and contains 
		/// methods to set and get properties of objects.
		/// </returns>
		public TableMapping GetMapping (Type type)
		{
			if (_mappings == null) {
				_mappings = new Dictionary<string, TableMapping> ();
			}
			TableMapping map;
			if (!_mappings.TryGetValue (type.FullName, out map)) {
				map = new TableMapping (type);
				_mappings [type.FullName] = map;
			}
			return map;
		}

		/// <summary>
		/// Executes a "create table if not exists" on the database. It also
		/// creates any specified indexes on the columns of the table. It uses
		/// a schema automatically generated from the specified type. You can
		/// later access this schema by calling GetMapping.
		/// </summary>
		/// <returns>
		/// The number of entries added to the database schema.
		/// </returns>
		public int CreateTable (Type ty)
		{
			if (_tables == null) {
				_tables = new Dictionary<string, TableMapping> ();
			}
			TableMapping map;
			if (!_tables.TryGetValue (ty.FullName, out map)) {
				map = GetMapping (ty);
				_tables.Add (ty.FullName, map);
			}
			var query = "create table \"" + map.TableName + "\"(\n";
			
			var decls = map.Columns.Select (p => Orm.SqlDecl (p));
			var decl = string.Join (",\n", decls.ToArray ());
			query += decl;
			query += ")";
			
			var count = 0;

			try {
				Execute (query);
				count = 1;
			}
			catch (SQLiteException) {
			}
			
			if (count == 0) {
				// Table already exists, migrate it
				MigrateTable (map);
			}

			foreach (var p in map.Columns.Where (x => x.IsIndexed)) {
				var indexName = map.TableName + "_" + p.Name;
				var q = string.Format ("create index if not exists \"{0}\" on \"{1}\"(\"{2}\")", indexName, map.TableName, p.Name);
				count += Execute (q);
			}
			
			return count;
		}

		class TableInfo
		{
			public int cid { get; set; }

			public string name { get; set; }

			public string type { get; set; }

			public int notnull { get; set; }

			public string dflt_value { get; set; }

			public int pk { get; set; }

            public int unique { get; set; }
		}

		void MigrateTable (TableMapping map)
		{
			var query = "pragma table_info(\"" + map.TableName + "\")";
			
			var existingCols = Query (typeof(TableInfo), query);

			var toBeAdded = new List<TableMapping.Column> ();
			
			foreach (var p in map.Columns) {
				var found = false;
				foreach (var o in existingCols)
				{
					var c = (TableInfo)o;
					found = p.Name == c.name;
					if (found)
						break;
				}
				if (!found) {
					toBeAdded.Add (p);
				}
			}
			
			foreach (var p in toBeAdded) {
				var addCol = "alter table \"" + map.TableName + "\" add column " + Orm.SqlDecl (p);
				Execute (addCol);
			}
		}

		/// <summary>
		/// Creates a new SQLiteCommand given the command text with arguments. Place a '?'
		/// in the command text for each of the arguments.
		/// </summary>
		/// <param name="cmdText">
		/// The fully escaped SQL.
		/// </param>
		/// <param name="args">
		/// Arguments to substitute for the occurences of '?' in the command text.
		/// </param>
		/// <returns>
		/// A <see cref="SQLiteCommand"/>
		/// </returns>
		public virtual SQLiteCommand CreateCommand (string cmdText, params object[] ps)
		{
			if (!_open) {
				throw SQLiteException.New (SQLite3.Result.Error, "Cannot create commands from unopened database");
			} else {
				var cmd = new SQLiteCommand (this);
				cmd.CommandText = cmdText;
				foreach (var o in ps) {
					cmd.Bind (o);
				}
				return cmd;
			}
		}

		/// <summary>
		/// Creates a SQLiteCommand given the command text (SQL) with arguments. Place a '?'
		/// in the command text for each of the arguments and then executes that command.
		/// Use this method instead of Query when you don't expect rows back. Such cases include
		/// INSERTs, UPDATEs, and DELETEs.
		/// You can set the Trace or TimeExecution properties of the connection
		/// to profile execution.
		/// </summary>
		/// <param name="query">
		/// The fully escaped SQL.
		/// </param>
		/// <param name="args">
		/// Arguments to substitute for the occurences of '?' in the query.
		/// </param>
		/// <returns>
		/// The number of rows modified in the database as a result of this execution.
		/// </returns>
		public int Execute (string query, params object[] args)
		{
			var cmd = CreateCommand (query, args);
			
			if (TimeExecution) {
				if (_sw == null) {
					_sw = new System.Diagnostics.Stopwatch ();
				}
				_sw.Reset ();
				_sw.Start ();
			}

            SQLite3.Result result;
            string errorMessage;
			int r = cmd.ExecuteNonQuery (out result, out errorMessage);
            if (result != SQLite3.Result.Done)
            {
                throw SQLiteException.New(result, errorMessage);
            }
			
			if (TimeExecution) {
				_sw.Stop ();
				_elapsedMilliseconds += _sw.ElapsedMilliseconds;
				Debug.Log ("Finished in " + _sw.ElapsedMilliseconds.ToString() + " ms (" + (_elapsedMilliseconds / 1000.0).ToString() + " s total)");
			}
			
			return r;
		}

        /// <summary>
        /// Creates a SQLiteCommand given the command text (SQL) with arguments. Place a '?'
        /// in the command text for each of the arguments and then executes that command.
        /// Use this method instead of Query when you don't expect rows back. Such cases include
        /// INSERTs, UPDATEs, and DELETEs.
        /// You can set the Trace or TimeExecution properties of the connection
        /// to profile execution.
        /// </summary>
        /// <param name="query">
        /// The fully escaped SQL.
        /// </param>
        /// <param name="args">
        /// Arguments to substitute for the occurences of '?' in the query.
        /// </param>
        /// <returns>
        /// The number of rows modified in the database as a result of this execution.
        /// </returns>
        public int Execute(out SQLite3.Result result, out string errorMessage, string query, params object[] args)
        {
            result = SQLite3.Result.OK;
            errorMessage = "";

            var cmd = CreateCommand(query, args);

            if (TimeExecution)
            {
                if (_sw == null)
                {
                    _sw = new System.Diagnostics.Stopwatch();
                }
                _sw.Reset();
                _sw.Start();
            }

            int r = cmd.ExecuteNonQuery(out result, out errorMessage);

            if (TimeExecution)
            {
                _sw.Stop();
                _elapsedMilliseconds += _sw.ElapsedMilliseconds;
                Debug.Log("Finished in " + _sw.ElapsedMilliseconds.ToString() + " ms (" + (_elapsedMilliseconds / 1000.0).ToString() + " s total)");
            }

            return r;
        }
		
		public List<object> Query (Type type, string query, params object[] args)
		{
			var cmd = CreateCommand (query, args);
			return cmd.ExecuteQuery(GetMapping(type));
		}

		/// <summary>
		/// Creates a SQLiteCommand given the command text (SQL) with arguments. Place a '?'
		/// in the command text for each of the arguments and then executes that command.
		/// It returns each row of the result using the specified mapping. This function is
		/// only used by libraries in order to query the database via introspection. It is
		/// normally not used.
		/// </summary>
		/// <param name="map">
		/// A <see cref="TableMapping"/> to use to convert the resulting rows
		/// into objects.
		/// </param>
		/// <param name="query">
		/// The fully escaped SQL.
		/// </param>
		/// <param name="args">
		/// Arguments to substitute for the occurences of '?' in the query.
		/// </param>
		/// <returns>
		/// An enumerable with one result for each row returned by the query.
		/// </returns>
		public List<object> Query (TableMapping map, string query, params object[] args)
		{
			var cmd = CreateCommand (query, args);
			return cmd.ExecuteQuery(map);
		}

		/// <summary>
		/// Attempts to retrieve an object with the given primary key from the table
		/// associated with the specified type. Use of this method requires that
		/// the given type have a designated PrimaryKey (using the PrimaryKeyAttribute).
		/// </summary>
		/// <param name="pk">
		/// The primary key.
		/// </param>
		/// <returns>
		/// The object with the given primary key. Throws a not found exception
		/// if the object is not found.
		/// </returns>
		public object Get (object pk, Type type)
		{
			var map = GetMapping (type);
			string query = string.Format ("select * from \"{0}\" where \"{1}\" = ?", map.TableName, map.PK.Name);
			return Query (type, query, pk).First ();
		}
		
		public object Find (object pk, Type type)
		{
			var map = GetMapping (type);
			string query = string.Format ("select * from \"{0}\" where \"{1}\" = ?", map.TableName, map.PK.Name);
			return Query (type, query, pk).FirstOrDefault();
		}

		/// <summary>
		/// Whether <see cref="BeginTransaction"/> has been called and the database is waiting for a <see cref="Commit"/>.
		/// </summary>
		public bool IsInTransaction { get; private set; }

		/// <summary>
		/// Begins a new transaction. Call <see cref="Commit"/> to end the transaction.
		/// </summary>
		public void BeginTransaction ()
		{
			if (!IsInTransaction) {
				Execute ("begin transaction");
				IsInTransaction = true;
			}
		}

		/// <summary>
		/// Rolls back the transaction that was begun by <see cref="BeginTransaction"/>.
		/// </summary>
		public void Rollback ()
		{
			if (IsInTransaction) {
				Execute ("rollback");
				IsInTransaction = false;
			}
		}

		/// <summary>
		/// Commits the transaction that was begun by <see cref="BeginTransaction"/>.
		/// </summary>
		public void Commit ()
		{
			if (IsInTransaction) {
				Execute ("commit");
				IsInTransaction = false;
			}
		}

		/// <summary>
		/// Executes <param name="action"> within a transaction and automatically rollsback the transaction
		/// if an exception occurs. The exception is rethrown.
		/// </summary>
		/// <param name="action">
		/// The <see cref="Action"/> to perform within a transaction. <param name="action"> can contain any number
		/// of operations on the connection but should never call <see cref="BeginTransaction"/>,
		/// <see cref="Rollback"/>, or <see cref="Commit"/>.
		/// </param>
		public void RunInTransaction (Action action)
		{
			if (IsInTransaction) {
				throw new InvalidOperationException ("The connection must not already be in a transaction when RunInTransaction is called");
			}
			try {
				BeginTransaction ();
				action ();
				Commit ();
			} catch (Exception) {
				Rollback ();
				throw;
			}
		}

		public int Insert (object obj, Type objType, out long rowID)
		{
			return Insert (obj, "", objType, out rowID);
		}

		public int InsertOrUpdate (object obj, Type objType, out long rowID)
		{
			return Insert (obj, "or replace", objType, out rowID);
		}

		/// <summary>
		/// Inserts the given object and retrieves its
		/// auto incremented primary key if it has one.
		/// </summary>
		/// <param name="obj">
		/// The object to insert.
		/// </param>
		/// <param name="extra">
		/// Literal SQL code that gets placed into the command. INSERT {extra} INTO ...
		/// </param>
        /// <param name="rowID">
        /// The underlying rowID returned from the insert
        /// </param>
        /// <returns>
		/// The number of rows added to the table.
		/// </returns>
		public int Insert (object obj, string extra, Type objType, out long rowID)
		{
            rowID = -1;

			if (obj == null || objType == null) {
				return 0;
			}
			
			var map = GetMapping (objType);
			
			var cols = map.InsertColumns;
			var vals = new object[cols.Length];
			for (var i = 0; i < vals.Length; i++) {
				vals [i] = cols [i].GetValue (obj);
			}

            var insertSql = map.InsertSql(extra);
            var count = Execute(insertSql, vals.ToArray());

			if (map.HasAutoIncPK) {
				rowID = SQLite3.LastInsertRowid (Handle);
				map.SetAutoIncPK (obj, rowID);
			}

			return count;
		}

		public int Update (object obj, Type objType)
		{
			if (obj == null || objType == null) {
				return 0;
			}
			
			var map = GetMapping (objType);
			
			var pk = map.PK;
			
			if (pk == null) {
				throw new NotSupportedException ("Cannot update " + map.TableName + ": it has no PK");
			}
			
			var cols = from p in map.Columns
				where p != pk
				select p;
			var vals = from c in cols
				select c.GetValue (obj);
			var ps = new List<object> (vals);
			ps.Add (pk.GetValue (obj));
			var q = string.Format ("update \"{0}\" set {1} where {2} = ? ", map.TableName, string.Join (",", (from c in cols
				select "\"" + c.Name + "\" = ? ").ToArray ()), pk.Name);
			return Execute (q, ps.ToArray ());
		}

		/// <summary>
		/// Deletes the given object from the database using its primary key.
		/// </summary>
		/// <param name="obj">
		/// The object to delete. It must have a primary key designated using the PrimaryKeyAttribute.
		/// </param>
		/// <param name="objType">objType</param>
		/// <returns>
		/// The number of rows deleted.
		/// </returns>
		public int Delete (object obj, Type objType)
		{
			var map = GetMapping (objType);
			var pk = map.PK;
			if (pk == null) {
				throw new NotSupportedException ("Cannot delete " + map.TableName + ": it has no PK");
			}
			var q = string.Format ("delete from \"{0}\" where \"{1}\" = ?", map.TableName, pk.Name);
			return Execute (q, pk.GetValue (obj));
		}
		
		public int DeleteByPK (object pkValue, Type objType)
		{
			var map = GetMapping (objType);
			var pk = map.PK;
			if (pk == null) {
				throw new NotSupportedException ("Cannot delete " + map.TableName + ": it has no PK");
			}
			var q = string.Format ("delete from \"{0}\" where \"{1}\" = ?", map.TableName, pk.Name);
			return Execute (q, pkValue);
		}

		public void Dispose ()
		{
			Close ();
		}

		public void Close ()
		{
			if (_open && Handle != IntPtr.Zero) {
				SQLite3.Close (Handle);
				Handle = IntPtr.Zero;
				_open = false;
			}
		}
	}

	public class PrimaryKeyAttribute : Attribute
	{
	}

    public class UniqueAttribute : Attribute
    {
    }

	public class AutoIncrementAttribute : Attribute
	{
	}

	public class IndexedAttribute : Attribute
	{
	}

	public class IgnoreAttribute : Attribute
	{
	}

	public class MaxLengthAttribute : Attribute
	{
		public int Value { get; private set; }

		public MaxLengthAttribute (int length)
		{
			Value = length;
		}
	}

	public class CollationAttribute: Attribute
	{
		public string Value { get; private set; }

		public CollationAttribute (string collation)
		{
			Value = collation;
		}
	}

    public class NotNullAttribute : Attribute
    {
    }

    public class DefaultAttribute : Attribute
    {
        public object Value { get; private set; }

        public DefaultAttribute(object obj)
        {
            Value = obj;
        }
    }   

	public class TableMapping
	{
		public Type MappedType { get; private set; }

		public string TableName { get; private set; }

		public Column[] Columns { get; private set; }

		public Column PK { get; private set; }

		Column _autoPk = null;
		Column[] _insertColumns = null;
		string _insertSql = null;

		public TableMapping (Type type)
		{
			MappedType = type;
			TableName = MappedType.Name;
			var props = MappedType.GetProperties (BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);
			var cols = new List<Column> ();
			foreach (var p in props) {
				var ignore = p.GetCustomAttributes (typeof(IgnoreAttribute), true).Length > 0;
				if (p.CanWrite && !ignore) {
					cols.Add (new PropColumn (p));
				}
			}
			Columns = cols.ToArray ();
			foreach (var c in Columns) {
				if (c.IsAutoInc && c.IsPK) {
					_autoPk = c;
				}
				if (c.IsPK) {
					PK = c;
				}
			}
			
			HasAutoIncPK = _autoPk != null;
		}

		public bool HasAutoIncPK { get; private set; }

		public void SetAutoIncPK (object obj, long id)
		{
			if (_autoPk != null) {
				_autoPk.SetValue (obj, Convert.ChangeType (id, _autoPk.ColumnType));
			}
		}

		public Column[] InsertColumns {
			get {
				if (_insertColumns == null) {
					_insertColumns = Columns.Where (c => !c.IsAutoInc).ToArray ();
				}
				return _insertColumns;
			}
		}

		public Column FindColumn (string name)
		{
			var exact = Columns.Where (c => c.Name == name).FirstOrDefault ();
			return exact;
		}

		public string InsertSql (string extra)
		{
			if (_insertSql == null) {
				var cols = InsertColumns;
				_insertSql = string.Format ("insert {3} into \"{0}\"({1}) values ({2})", TableName, string.Join (",", (from c in cols
					select "\"" + c.Name + "\"").ToArray ()), string.Join (",", (from c in cols
					select "?").ToArray ()), extra);
			}
			return _insertSql;
		}

		PreparedSqlLiteInsertCommand _insertCommand;
		string _insertCommandExtra = null;

		public PreparedSqlLiteInsertCommand GetInsertCommand (SQLiteConnection conn, string extra)
		{
			if (_insertCommand == null || _insertCommandExtra != extra) {
				var insertSql = InsertSql (extra);
				_insertCommand = new PreparedSqlLiteInsertCommand (conn);
				_insertCommand.CommandText = insertSql;
				_insertCommandExtra = extra;
			}
			return _insertCommand;
		}

		public abstract class Column
		{
			public string Name { get; protected set; }

			public Type ColumnType { get; protected set; }

			public string Collation { get; protected set; }

			public bool IsAutoInc { get; protected set; }

			public bool IsPK { get; protected set; }

            public bool IsUnique { get; protected set; }

			public bool IsIndexed { get; protected set; }

			public bool IsNullable { get; protected set; }

			public int MaxStringLength { get; protected set; }

            public object Default { get; protected set; }

			public abstract void SetValue (object obj, object val);

			public abstract object GetValue (object obj);
		}

		public class PropColumn : Column
		{
			PropertyInfo _prop;

			public PropColumn (PropertyInfo prop)
			{
				_prop = prop;
				Name = prop.Name;
				//If this type is Nullable<T> then Nullable.GetUnderlyingType returns the T, otherwise it returns null, so get the the actual type instead
				ColumnType = RuntimeHelper.GetPropertyTypeFunc(prop);
				Collation = Orm.Collation (prop);
				IsAutoInc = Orm.IsAutoInc (prop);
				IsPK = Orm.IsPK (prop);
                IsUnique = Orm.IsUnique (prop);
				IsIndexed = Orm.IsIndexed (prop);
				IsNullable = !IsPK && !Orm.IsNotNull(prop);
				MaxStringLength = Orm.MaxStringLength (prop);
                Default = Orm.Default(prop);
			}

			public override void SetValue (object obj, object val)
			{
				_prop.SetValue (obj, val, null);
			}

			public override object GetValue (object obj)
			{
				return _prop.GetValue (obj, null);
			}
		}
	}

	public static class Orm
	{
		public const int DefaultMaxStringLength = 140;

		public static string SqlDecl (TableMapping.Column p)
		{
			string decl = "\"" + p.Name + "\" " + SqlType (p) + " ";

			if (p.IsPK) {
				decl += "primary key ";
			}
            if (p.IsUnique)
            {
                decl += "unique ";
            }
			if (p.IsAutoInc) {
				decl += "autoincrement ";
			}
			if (!p.IsNullable) {
				decl += "not null ";
			}
			if (!string.IsNullOrEmpty (p.Collation)) {
				decl += "collate " + p.Collation + " ";
			}
            if (!string.IsNullOrEmpty(p.Default.ToString()))
            {
                decl += "default " + p.Default.ToString() + " ";
            }

			return decl;
		}

		public static string SqlType (TableMapping.Column p)
		{
			var clrType = p.ColumnType;
			if (clrType == typeof(Boolean) || clrType == typeof(Byte) || clrType == typeof(UInt16) || clrType == typeof(SByte) || clrType == typeof(Int16) || clrType == typeof(Int32)) {
				return "integer";
			} else if (clrType == typeof(UInt32) || clrType == typeof(Int64)) {
				return "bigint";
			} else if (clrType == typeof(Single) || clrType == typeof(Double) || clrType == typeof(Decimal)) {
				return "float";
			} else if (clrType == typeof(String)) {
				int len = p.MaxStringLength;
				return "varchar(" + len + ")";
			} else if (clrType == typeof(DateTime)) {
				return "datetime";
			} else if (clrType.IsEnum) {
				return "integer";
			} else if (clrType == typeof(byte[])) {
				return "blob";
			} else {
				throw new NotSupportedException ("Don't know about " + clrType);
			}
		}

		public static bool IsPK (MemberInfo p)
		{
			var attrs = p.GetCustomAttributes (typeof(PrimaryKeyAttribute), true);
			return attrs.Length > 0;
		}

        public static bool IsUnique(MemberInfo p)
        {
            var attrs = p.GetCustomAttributes(typeof(UniqueAttribute), true);
            return attrs.Length > 0;
        }

		public static string Collation (MemberInfo p)
		{
			var attrs = p.GetCustomAttributes (typeof(CollationAttribute), true);
			if (attrs.Length > 0) {
				return ((CollationAttribute)attrs [0]).Value;
			} else {
				return string.Empty;
			}
		}

		public static bool IsAutoInc (MemberInfo p)
		{
			var attrs = p.GetCustomAttributes (typeof(AutoIncrementAttribute), true);
			return attrs.Length > 0;
		}

		public static bool IsIndexed (MemberInfo p)
		{
			var attrs = p.GetCustomAttributes (typeof(IndexedAttribute), true);
			return attrs.Length > 0;
		}

		public static int MaxStringLength (PropertyInfo p)
		{
			var attrs = p.GetCustomAttributes (typeof(MaxLengthAttribute), true);
			if (attrs.Length > 0) {
				return ((MaxLengthAttribute)attrs [0]).Value;
			} else {
				return DefaultMaxStringLength;
			}
		}

        public static bool IsNotNull(MemberInfo p)
        {
            var attrs = p.GetCustomAttributes(typeof(NotNullAttribute), true);
            return attrs.Length > 0;
        }

        public static object Default(MemberInfo p)
        {
            var attrs = p.GetCustomAttributes(typeof(DefaultAttribute), true);
            if (attrs.Length > 0)
            {
                return ((DefaultAttribute)attrs[0]).Value;
            }
            else
            {
                return string.Empty;
            }
        }
	}

	public class SQLiteCommand
	{
		protected SQLiteConnection _conn;
		protected List<Binding> _bindings;

		public string CommandText { get; set; }

		public SQLiteCommand (SQLiteConnection conn)
		{
			_conn = conn;
			_bindings = new List<Binding> ();
			CommandText = "";
		}

		public int ExecuteNonQuery (out SQLite3.Result result, out string errorMessage)
		{
            result = SQLite3.Result.OK;
            errorMessage = "";

			if (_conn.Trace) {
				Debug.Log ("Executing: " + this);
			}

			var stmt = Prepare2 (out result, out errorMessage);
            if (result != SQLite3.Result.OK)
            {
                return 0;
            }
			result = SQLite3.Step (stmt);
			Finalize (stmt);
			if (result == SQLite3.Result.Done) 
            {
				int rowsAffected = SQLite3.Changes (_conn.Handle);
				return rowsAffected;
            }
            else if (result == SQLite3.Result.Error)
            {
				errorMessage = SQLite3.GetErrmsg (_conn.Handle);
                return 0;
				//throw SQLiteException.New (r, msg);
			} 
            else 
            {
                errorMessage = result.ToString();
                return 0;
				//throw SQLiteException.New (r, r.ToString ());
			}
		}

		public List<object> ExecuteQuery (TableMapping map)
		{
			if (_conn.Trace) {
				Debug.Log ("Executing Query: " + this);
			}
			
			var r = new List<object> ();

			var stmt = Prepare3 ();
			
			var cols = new TableMapping.Column[SQLite3.ColumnCount (stmt)];
			
			for (int i = 0; i < cols.Length; i++) {
				var name = Marshal.PtrToStringUni (SQLite3.ColumnName16 (stmt, i));
				cols [i] = map.FindColumn (name);
			}
			
			while (SQLite3.Step (stmt) == SQLite3.Result.Row) {
				var obj = RuntimeHelper.CreateInstance (map.MappedType);
				for (int i = 0; i < cols.Length; i++) {
					if (cols [i] == null)
						continue;
					var colType = SQLite3.ColumnType (stmt, i);
					var val = ReadCol (stmt, i, colType, cols [i].ColumnType);
					cols [i].SetValue (obj, val);
				}
				r.Add (obj);
			}
			
			Finalize (stmt);
			return r;
		}

        public SimpleDataTable ExecuteQueryGeneric()
        {
            if (_conn.Trace)
            {
                Debug.Log("Executing Query: " + this);
            }

            var stmt = Prepare3();

            SimpleDataTable dt = new SimpleDataTable();
            SimpleDataRow dr;
            var columnCount = SQLite3.ColumnCount(stmt);
            Type columnType;

            for (int c = 0; c < columnCount; c++)
            {
                dt.columns.Add(new SimpleDataColumn() 
                { 
                    name = Marshal.PtrToStringUni(SQLite3.ColumnName16(stmt, c)) 
                });
            }

            while (SQLite3.Step(stmt) == SQLite3.Result.Row)
            {
                dr = dt.NewRow();

                for (int i = 0; i < columnCount; i++)
                {
                    columnType = GetDataType(SQLite3.ColumnType(stmt, i).ToString());

                    if (columnType == typeof(Int32))
                    {
                        dr[i] = SQLite3.ColumnInt(stmt, i);
                    }
                    else if (columnType == typeof(Int64))
                    {
                        dr[i] = SQLite3.ColumnInt64(stmt, i);
                    }
                    else if (columnType == typeof(double))
                    {
                        dr[i] = SQLite3.ColumnDouble(stmt, i);
                    }
                    else if (columnType == typeof(string))
                    {
                        dr[i] = SQLite3.ColumnString(stmt, i);
                    }
                    else if (columnType == typeof(DateTime))
                    {
                        dr[i] = Convert.ToDateTime(SQLite3.ColumnString(stmt, i));
                    }
                    else if (columnType == typeof(byte[]))
                    {
                        dr[i] = SQLite3.ColumnByteArray(stmt, i);
                    }
                    else if (columnType == typeof(object))
                    {
                        dr[i] = SQLite3.ColumnString(stmt, i);
                    }
                    else
                    {
                        dr[i] = null;
                    }
                }
            }

            Finalize(stmt);

            return dt;
        }

        private static Type GetDataType(string sqlColType)
        {
            sqlColType = sqlColType.Trim().ToLower();

            if (sqlColType == "integer")
            {
                return typeof(Int32);
            }
            else if (sqlColType == "bigint")
            {
                return typeof(Int64);
            }
            else if (sqlColType == "float")
            {
                return typeof(double);
            }
            else if (sqlColType.Contains("varchar"))
            {
                return typeof(string);
            }
            else if (sqlColType == "datetime")
            {
                return typeof(DateTime);
            }
            else if (sqlColType == "blob")
            {
                return typeof(byte[]);
            }
            else if (sqlColType == "text")
            {
                return typeof(string);
            }
            else if (sqlColType == "null")
            {
                return typeof(object);
            }
            else
            {
                return typeof(string);
            }
        }

		public T ExecuteScalar<T> ()
		{
			if (_conn.Trace) {
				Debug.Log ("Executing Query: " + this);
			}
			
			T val = default(T);
			
			var stmt = Prepare3 ();
			if (SQLite3.Step (stmt) == SQLite3.Result.Row) {
				var colType = SQLite3.ColumnType (stmt, 0);
				val = (T)ReadCol (stmt, 0, colType, typeof(T));
			}
			Finalize (stmt);
			
			return val;
		}

		public void Bind (string name, object val)
		{
			_bindings.Add (new Binding {
				Name = name,
				Value = val
			});
		}

		public void Bind (object val)
		{
			Bind (null, val);
		}

		public override string ToString ()
		{
			var parts = new string[1 + _bindings.Count];
			parts [0] = CommandText;
			var i = 1;
			foreach (var b in _bindings) {
				parts [i] = string.Format ("  {0}: {1}", i - 1, b.Value);
				i++;
			}
			return string.Join (Environment.NewLine, parts);
		}

		protected IntPtr Prepare2 (out SQLite3.Result result, out string errorMessage)
		{
			var stmt = SQLite3.Prepare2 (out result, out errorMessage, _conn.Handle, CommandText);
			BindAll (stmt);
			return stmt;
		}

        protected IntPtr Prepare3()
        {
            var stmt = SQLite3.Prepare3(_conn.Handle, CommandText);
            BindAll(stmt);
            return stmt;
        }

        protected void Finalize(IntPtr stmt)
		{
			SQLite3.Finalize (stmt);
		}

        protected void BindAll(IntPtr stmt)
		{
			int nextIdx = 1;
			foreach (var b in _bindings) {
				if (b.Name != null) {
					b.Index = SQLite3.BindParameterIndex (stmt, b.Name);
				} else {
					b.Index = nextIdx++;
				}
			}
			foreach (var b in _bindings) {
				BindParameter (stmt, b.Index, b.Value);
			}
		}

		internal static IntPtr NegativePointer = new IntPtr (-1);

		internal static void BindParameter (IntPtr stmt, int index, object value)
		{
			if (value == null) {
				SQLite3.BindNull (stmt, index);
			} else {
				if (value is Int32) {
					SQLite3.BindInt (stmt, index, (int)value);
				} else if (value is String) {
					SQLite3.BindText (stmt, index, (string)value, -1, NegativePointer);
				} else if (value is Byte || value is UInt16 || value is SByte || value is Int16) {
					SQLite3.BindInt (stmt, index, Convert.ToInt32 (value));
				} else if (value is Boolean) {
					SQLite3.BindInt (stmt, index, (bool)value ? 1 : 0);
				} else if (value is UInt32 || value is Int64) {
					SQLite3.BindInt64 (stmt, index, Convert.ToInt64 (value));
				} else if (value is Single || value is Double || value is Decimal) {
					SQLite3.BindDouble (stmt, index, Convert.ToDouble (value));
				} else if (value is DateTime) {
					SQLite3.BindText (stmt, index, ((DateTime)value).ToString ("yyyy-MM-dd HH:mm:ss"), -1, NegativePointer);
				} else if (value.GetType ().IsEnum) {
					SQLite3.BindInt (stmt, index, Convert.ToInt32 (value));
				} else if (value is byte[]) {
					SQLite3.BindBlob (stmt, index, (byte[])value, ((byte[])value).Length, NegativePointer);
				} else {
					throw new NotSupportedException ("Cannot store type: " + value.GetType ());
				}
			}
		}

        protected class Binding
		{
			public string Name { get; set; }

			public object Value { get; set; }

			public int Index { get; set; }
		}

		object ReadCol (IntPtr stmt, int index, SQLite3.ColType type, Type clrType)
		{
			if (type == SQLite3.ColType.Null) {
				return null;
			} else {
				if (clrType == typeof(String)) {
					return SQLite3.ColumnString (stmt, index);
				} else if (clrType == typeof(Int32)) {
					return (int)SQLite3.ColumnInt (stmt, index);
				} else if (clrType == typeof(Boolean)) {
					return SQLite3.ColumnInt (stmt, index) == 1;
				} else if (clrType == typeof(double)) {
					return SQLite3.ColumnDouble (stmt, index);
				} else if (clrType == typeof(float)) {
					return (float)SQLite3.ColumnDouble (stmt, index);
				} else if (clrType == typeof(DateTime)) {
					var text = SQLite3.ColumnString (stmt, index);
					return DateTime.Parse (text);
				} else if (clrType.IsEnum) {
					return SQLite3.ColumnInt (stmt, index);
				} else if (clrType == typeof(Int64)) {
					return SQLite3.ColumnInt64 (stmt, index);
				} else if (clrType == typeof(UInt32)) {
					return (uint)SQLite3.ColumnInt64 (stmt, index);
				} else if (clrType == typeof(decimal)) {
					return (decimal)SQLite3.ColumnDouble (stmt, index);
				} else if (clrType == typeof(Byte)) {
					return (byte)SQLite3.ColumnInt (stmt, index);
				} else if (clrType == typeof(UInt16)) {
					return (ushort)SQLite3.ColumnInt (stmt, index);
				} else if (clrType == typeof(Int16)) {
					return (short)SQLite3.ColumnInt (stmt, index);
				} else if (clrType == typeof(sbyte)) {
					return (sbyte)SQLite3.ColumnInt (stmt, index);
				} else if (clrType == typeof(byte[])) {
					return SQLite3.ColumnByteArray (stmt, index);
				} else {
					throw new NotSupportedException ("Don't know how to read " + clrType);
				}
			}
		}
	}

	/// <summary>
	/// Since the insert never changed, we only need to prepare once.
	/// </summary>
	public class PreparedSqlLiteInsertCommand : IDisposable
	{
		public bool Initialized { get; set; }

		protected SQLiteConnection Connection { get; set; }

		public string CommandText { get; set; }

		protected IntPtr Statement { get; set; }

		internal PreparedSqlLiteInsertCommand (SQLiteConnection conn)
		{
			Connection = conn;
		}

		public int ExecuteNonQuery (out SQLite3.Result result, out string errorMessage, object[] source)
		{
            result = SQLite3.Result.OK;
            errorMessage = "";

			if (Connection.Trace) {
				Debug.Log ("Executing: " + CommandText);
			}

            if (!Initialized)
            {
				Statement = Prepare2 (out result, out errorMessage);
                if (result != SQLite3.Result.OK)
                {
                    return 0;
                }
				Initialized = true;
			}

			//bind the values.
			if (source != null) {
				for (int i = 0; i < source.Length; i++) {
					SQLiteCommand.BindParameter (Statement, i + 1, source [i]);
				}
			}
			result = SQLite3.Step (Statement);

			if (result == SQLite3.Result.Done) 
            {
				int rowsAffected = SQLite3.Changes (Connection.Handle);
				SQLite3.Reset (Statement);
				return rowsAffected;
			} 
            else if (result == SQLite3.Result.Error) 
            {
				errorMessage = SQLite3.GetErrmsg (Connection.Handle);
				SQLite3.Reset (Statement);
                return 0;
				//throw SQLiteException.New (r, msg);
			} 
            else 
            {
                errorMessage = result.ToString();
				SQLite3.Reset (Statement);
                return 0;
				//throw SQLiteException.New (r, r.ToString ());
			}
		}

		protected virtual IntPtr Prepare2 (out SQLite3.Result result, out string errorMessage)
		{
			var stmt = SQLite3.Prepare2 (out result, out errorMessage, Connection.Handle, CommandText);
			return stmt;
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		private void Dispose (bool disposing)
		{
			if (Statement != IntPtr.Zero) {
				try {
					SQLite3.Finalize (Statement);
				} finally {
					Statement = IntPtr.Zero;
					Connection = null;
				}
			}
		}

		~PreparedSqlLiteInsertCommand ()
		{
			Dispose (false);
		}
	}

    public static class SQLite3
    {
        private const string dllPath = "sqlite3"; // "__internal";

        public enum Result : int
        {
            OK = 0,
            Error = 1,
            Internal = 2,
            Perm = 3,
            Abort = 4,
            Busy = 5,
            Locked = 6,
            NoMem = 7,
            ReadOnly = 8,
            Interrupt = 9,
            IOError = 10,
            Corrupt = 11,
            NotFound = 12,
            TooBig = 18,
            Constraint = 19,
            Row = 100,
            Done = 101
        }

        public enum ConfigOption : int
        {
            SingleThread = 1,
            MultiThread = 2,
            Serialized = 3
        }

        [DllImport(dllPath, EntryPoint = "sqlite3_open")]
        public static extern Result Open(string filename, out IntPtr db);

        [DllImport(dllPath, EntryPoint = "sqlite3_close")]
        public static extern Result Close(IntPtr db);

        [DllImport(dllPath, EntryPoint = "sqlite3_config")]
        public static extern Result Config(ConfigOption option);

        [DllImport(dllPath, EntryPoint = "sqlite3_busy_timeout")]
        public static extern Result BusyTimeout(IntPtr db, int milliseconds);

        [DllImport(dllPath, EntryPoint = "sqlite3_changes")]
        public static extern int Changes(IntPtr db);

        [DllImport(dllPath, EntryPoint = "sqlite3_prepare_v2")]
        public static extern Result Prepare2(IntPtr db, string sql, int numBytes, out IntPtr stmt, IntPtr pzTail);

        public static IntPtr Prepare2(out SQLite3.Result result, out string errorMessage, IntPtr db, string query)
        {
            result = SQLite3.Result.OK;
            errorMessage = "";

            IntPtr stmt;
            result = Prepare2(db, query, query.Length, out stmt, IntPtr.Zero);
            if (result != Result.OK)
            {
                errorMessage = GetErrmsg(db);
            }
            return stmt;
        }

        public static IntPtr Prepare3(IntPtr db, string query)
        {
            IntPtr stmt;
            var r = Prepare2(db, query, query.Length, out stmt, IntPtr.Zero);
            if (r != Result.OK)
            {
                throw SQLiteException.New(r, GetErrmsg(db));
            }
            return stmt;
        }

        [DllImport(dllPath, EntryPoint = "sqlite3_step")]
        public static extern Result Step(IntPtr stmt);

        [DllImport(dllPath, EntryPoint = "sqlite3_reset")]
        public static extern Result Reset(IntPtr stmt);

        [DllImport(dllPath, EntryPoint = "sqlite3_finalize")]
        public static extern Result Finalize(IntPtr stmt);

        [DllImport(dllPath, EntryPoint = "sqlite3_last_insert_rowid")]
        public static extern long LastInsertRowid(IntPtr db);

        [DllImport(dllPath, EntryPoint = "sqlite3_errmsg16")]
        public static extern IntPtr Errmsg(IntPtr db);

        public static string GetErrmsg(IntPtr db)
        {
            return Marshal.PtrToStringUni(Errmsg(db));
        }

        [DllImport(dllPath, EntryPoint = "sqlite3_bind_parameter_index")]
        public static extern int BindParameterIndex(IntPtr stmt, string name);

        [DllImport(dllPath, EntryPoint = "sqlite3_bind_null")]
        public static extern int BindNull(IntPtr stmt, int index);

        [DllImport(dllPath, EntryPoint = "sqlite3_bind_int")]
        public static extern int BindInt(IntPtr stmt, int index, int val);

        [DllImport(dllPath, EntryPoint = "sqlite3_bind_int64")]
        public static extern int BindInt64(IntPtr stmt, int index, long val);

        [DllImport(dllPath, EntryPoint = "sqlite3_bind_double")]
        public static extern int BindDouble(IntPtr stmt, int index, double val);

        [DllImport(dllPath, EntryPoint = "sqlite3_bind_text")]
        public static extern int BindText(IntPtr stmt, int index, string val, int n, IntPtr free);

        [DllImport(dllPath, EntryPoint = "sqlite3_bind_blob")]
        public static extern int BindBlob(IntPtr stmt, int index, byte[] val, int n, IntPtr free);

        [DllImport(dllPath, EntryPoint = "sqlite3_column_count")]
        public static extern int ColumnCount(IntPtr stmt);

        [DllImport(dllPath, EntryPoint = "sqlite3_column_name")]
        public static extern IntPtr ColumnName(IntPtr stmt, int index);

        [DllImport(dllPath, EntryPoint = "sqlite3_column_name16")]
        public static extern IntPtr ColumnName16(IntPtr stmt, int index);

        [DllImport(dllPath, EntryPoint = "sqlite3_column_origin_name")]
        public static extern string ColumnOriginName(IntPtr stmt, int index);

        [DllImport(dllPath, EntryPoint = "sqlite3_column_type")]
        public static extern ColType ColumnType(IntPtr stmt, int index);

        [DllImport(dllPath, EntryPoint = "sqlite3_column_int")]
        public static extern int ColumnInt(IntPtr stmt, int index);

        [DllImport(dllPath, EntryPoint = "sqlite3_column_int64")]
        public static extern long ColumnInt64(IntPtr stmt, int index);

        [DllImport(dllPath, EntryPoint = "sqlite3_column_double")]
        public static extern double ColumnDouble(IntPtr stmt, int index);

        [DllImport(dllPath, EntryPoint = "sqlite3_column_text")]
        public static extern IntPtr ColumnText(IntPtr stmt, int index);

        [DllImport(dllPath, EntryPoint = "sqlite3_column_text16")]
        public static extern IntPtr ColumnText16(IntPtr stmt, int index);

        [DllImport(dllPath, EntryPoint = "sqlite3_column_blob")]
        public static extern IntPtr ColumnBlob(IntPtr stmt, int index);

        [DllImport(dllPath, EntryPoint = "sqlite3_column_bytes")]
        public static extern int ColumnBytes(IntPtr stmt, int index);

        public static string ColumnString(IntPtr stmt, int index)
        {
            return Marshal.PtrToStringUni(SQLite3.ColumnText16(stmt, index));
        }

        public static byte[] ColumnByteArray(IntPtr stmt, int index)
        {
            int length = ColumnBytes(stmt, index);
            byte[] result = new byte[length];
            if (length > 0)
                Marshal.Copy(ColumnBlob(stmt, index), result, 0, length);
            return result;
        }

        public enum ColType : int
        {
            Integer = 1,
            Float = 2,
            Text = 3,
            Blob = 4,
            Null = 5
        }
    }
	
}