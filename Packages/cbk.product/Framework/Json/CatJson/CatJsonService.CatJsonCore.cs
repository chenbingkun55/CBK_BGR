using System;
using CatJson;
using CBK.Framework.Reference;

namespace CBK.Framework.Json
{
    internal sealed partial class CatJsonService
    {
        private sealed class CatJsonCore : AReference, IJsonCore
        {
            private readonly JsonParser m_JsonParser = new JsonParser()
            {
                IsPolymorphic = false,
                IgnoreDefaultValue = false
            };

            public void Dispose()
            {
                this.Recycle();
            }

            public override void OnRecycle()
            {
            }

            public string Serialize<T>(T obj)
            {
                try
                {
                    return m_JsonParser.ToJson(obj);
                }
                catch (Exception e)
                {
                    Log.Exception(e);
                    return string.Empty;
                }
            }

            public T Deserialize<T>(string json)
            {
                try
                {
                    return m_JsonParser.ParseJson<T>(json);
                }
                catch (Exception e)
                {
                    Log.Exception(e);
                    return default;
                }
            }
        }
    }
}