using System;
using System.Collections.Generic;

namespace CatJson
{
    public class ObjectFormatter : BaseJsonFormatter<object>
    {
        public override void ToJson(JsonParser parser, object value, Type type, Type realType, int depth)
        {
            throw new NotImplementedException();
        }

        public override object ParseJson(JsonParser parser, Type type, Type realType)
        {
            //这里只能look不能get，get交给各类型的formatter去进行
            TokenType nextTokenType = parser.Lexer.LookNextTokenType();
            
            switch (nextTokenType)
            {
                case TokenType.Null:
                    parser.Lexer.GetNextTokenByType(TokenType.Null);
                    return null;
                
                case TokenType.True:
                case TokenType.False:
                    return parser.ParseJson<bool>();
                
                case TokenType.Number:
                    return parser.ParseJson<double>();
                
                case TokenType.String:
                    return parser.ParseJson<string>();
                
                case TokenType.LeftBracket:
                    return parser.ParseJson<List<object>>();
                
                case TokenType.LeftBrace:
                    return parser.ParseJson<Dictionary<string, object>>();
                default:
                    throw new Exception("Object解析失败，tokenType == " + nextTokenType);
            }

        }
    }
}