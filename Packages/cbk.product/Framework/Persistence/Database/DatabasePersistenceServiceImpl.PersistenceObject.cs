using CBK.Framework.Reference;
using SimpleSQL;

namespace CBK.Framework.Persistence.Database
{
    internal partial class DatabasePersistenceServiceImpl
    {
        private sealed class PersistenceObject : AReference
        {
            [PrimaryKey]
            public string Key { get; set; }
            
            public string StringValue { get; set; }
            
            public float FloatValue { get; set; }
            
            public int IntValue { get; set; }
            
            public override void OnRecycle()
            {
                Key = null;
                StringValue = null;
                FloatValue = 0;
                IntValue = 0;
            }
        }
    }
}