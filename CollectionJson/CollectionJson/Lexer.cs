using System.Collections;
using System.Text;

namespace CollectionJson
{
    public class Lexer
    {
        public static List<JsonToken> Lex(string json)
        {
            var jsonStream = new JsonReader(json);
            var cJsonTokens = new List<JsonToken>();

            while (!jsonStream.Peek(out char c))
            {
                cJsonTokens.Add(c switch
                {
                    '{' => jsonStream.ReadToken(c, TokenType.OpenCurly),
                    '}' => jsonStream.ReadToken(c, TokenType.CloseCurly),
                    '[' => jsonStream.ReadToken(c, TokenType.OpenSquare),
                    ']' => jsonStream.ReadToken(c, TokenType.CloseSquare),
                    ':' => jsonStream.ReadToken(c, TokenType.Colon),
                    ',' => jsonStream.ReadToken(c, TokenType.Comma),
                    '\"' => jsonStream.ReadString(),
                    'n' => jsonStream.ReadToken("null", TokenType.Null),
                    'f' => jsonStream.ReadToken("false", TokenType.False),
                    't' => jsonStream.ReadToken("true", TokenType.True),
                    var n when char.IsDigit(n) => jsonStream.ReadNumber(),
                    var w when char.IsWhiteSpace(w) => jsonStream.ReadWhiteSpace(),
                    _ => throw new LexerException($"unexpected character in input: {c}"),
                });
            }

            cJsonTokens.Add(new JsonToken("", TokenType.EndOfStreamToken));
            return cJsonTokens;
        }

        private static JsonToken ReadString2(JsonReader jsonStream)
        {
            return ParseNextToken2(jsonStream, (StringBuilder buffer, char c, out TokenType tokenType) =>
            {
                if (c != '"')
                {
                    buffer.Append(jsonStream.Read().c);
                    tokenType = TokenType.Indeterminate;
                }
                else
                {
                    buffer.Append(jsonStream.Consume("\""));
                    tokenType = TokenType.String;
                }
            });
        }

        public static JsonToken ParseNextToken(JsonReader jsonStream, Func<StringBuilder, char, TokenType> body)
        {
            var buffer = new StringBuilder(jsonStream.Consume("\""));
            TokenType tokenType = TokenType.Indeterminate;

            while (tokenType == TokenType.Indeterminate && !jsonStream.Peek(out char c))
            {
                tokenType = body(buffer, c);
            }

            return new JsonToken(buffer.ToString(), tokenType);
        }

        public delegate void Body<T1, T2, T3>(T1 v1, T2 v2, out T3 v3);

        public static JsonToken ParseNextToken2(JsonReader jsonStream, Body<StringBuilder, char, TokenType> body)
        {
            var buffer = new StringBuilder(jsonStream.Consume("\""));
            TokenType tokenType = TokenType.Indeterminate;

            while (tokenType == TokenType.Indeterminate && !jsonStream.Peek(out char c))
            {
                body(buffer, c, out tokenType);
            }

            return new JsonToken(buffer.ToString(), tokenType);
        }
    }
}