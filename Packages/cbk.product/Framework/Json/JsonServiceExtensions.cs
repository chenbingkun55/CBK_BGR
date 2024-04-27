namespace CBK.Framework.Json
{
    public static class JsonServiceExtensions
    {
        public static string ToJson<T>(this T obj, IJsonCore jsonCore = null)
        {
            return jsonCore != null ? jsonCore.Serialize(obj) : JsonService.That.Serialize(obj);
        }
        
        public static T FromJson<T>(this string json, IJsonCore jsonCore = null)
        {
            return jsonCore != null ? jsonCore.Deserialize<T>(json) : JsonService.That.Deserialize<T>(json);
        }
    }
}