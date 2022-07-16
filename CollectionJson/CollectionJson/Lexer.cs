using System.Text;

namespace CollectionJson
{
    public class Lexer
    {
        public static List<CJsonToken> Lex(string json)
        {
            var jsonStream = new CJsonReader(json);
            var cJsonTokens = new List<CJsonToken>();

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
                    _ => throw new CJsonLexerException($"unexpected character in input: {c}"),
                });
            }
            return cJsonTokens;
        }

        private static CJsonToken ReadString2(CJsonReader jsonStream)
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

        public static CJsonToken ParseNextToken(CJsonReader jsonStream, Func<StringBuilder, char, TokenType> body)
        {
            var buffer = new StringBuilder(jsonStream.Consume("\""));
            TokenType tokenType = TokenType.Indeterminate;

            while (tokenType == TokenType.Indeterminate && !jsonStream.Peek(out char c))
            {
                tokenType = body(buffer, c);
            }

            return new CJsonToken(buffer.ToString(), tokenType);
        }

        public delegate void Body<T1, T2, T3>(T1 v1, T2 v2, out T3 v3);

        public static CJsonToken ParseNextToken2(CJsonReader jsonStream, Body<StringBuilder, char, TokenType> body)
        {
            var buffer = new StringBuilder(jsonStream.Consume("\""));
            TokenType tokenType = TokenType.Indeterminate;

            while (tokenType == TokenType.Indeterminate && !jsonStream.Peek(out char c))
            {
                body(buffer, c, out tokenType);
            }

            return new CJsonToken(buffer.ToString(), tokenType);
        }
    }
}