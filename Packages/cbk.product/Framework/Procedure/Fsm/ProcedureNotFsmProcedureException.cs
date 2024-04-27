using System;

namespace CBK.Framework.Procedure.Fsm
{
    public class ProcedureNotFsmProcedureException : System.Exception
    {
        public Type Type { get; }

        public ProcedureNotFsmProcedureException(Type type)
        {
            Type = type;
        }
    }
}