using System;

namespace CatJson
{
    /// <summary>
    /// 枚举类型的Json格式化器
    /// </summary>
    public class EnumFormatter : IJsonFormatter
    {
        /// <inheritdoc />
        public void ToJson(JsonParser parser, object value, Type type, Type realType, int depth)
        {
            parser.Append(Convert.ToInt32(value).ToString());
        }

        /// <inheritdoc />
        public object ParseJson(JsonParser parser, Type type, Type realType)
        {
            RangeString rs = parser.Lexer.GetNextTokenByType(TokenType.Number);
            return Enum.ToObject(realType,rs.AsInt());
        }
    }
}