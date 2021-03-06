using System.Text;
using CollectionJson;
using Newtonsoft.Json.Linq;

namespace Test.CollectionJson
{
    public class LexerTests
    {
        [Theory]
        [InlineData("{", TokenType.OpenCurly)]
        [InlineData("}", TokenType.CloseCurly)]
        [InlineData("[", TokenType.OpenSquare)]
        [InlineData("]", TokenType.CloseSquare)]
        [InlineData(":", TokenType.Colon)]
        [InlineData("\"\"", TokenType.String)]
        [InlineData("\"This is a string\"", TokenType.String)]
        [InlineData("123", TokenType.Integer)]
        [InlineData("123.123", TokenType.Decimal)]
        [InlineData("null", TokenType.Null)]
        [InlineData("false", TokenType.False)]
        [InlineData("true", TokenType.True)]
        [InlineData(",", TokenType.Comma)]
        public void Lexer_SingleTokenTests(string json, TokenType tokenType)
        {
            List<JsonToken> tokens = Lexer.Lex(json);
            var actualToken = tokens.First();
            AssertToken(json, tokenType, actualToken);
        }

        [Theory]
        [InlineData(@"""\""""", @"""""""")]
        [InlineData(@"""\\""", "\"\\\"")]
        [InlineData(@"""\/""", "\"/\"")]
        [InlineData(@"""\f""", "\"\f\"")]
        [InlineData(@"""\n""", "\"\n\"")]
        [InlineData(@"""\r""", "\"\r\"")]
        [InlineData(@"""\t""", "\"\t\"")]
        public void Lexer_StringEscapeSequence(string json, string expected)
        {
            List<JsonToken> tokens = Lexer.Lex(json);
            var actualToken = tokens.First();
            Assert.Equal(expected, actualToken.Token);
        }

        [Fact]
        public void Lexer_InvalidEscapeSequence_ThrowsException()
        {
            Assert.Throws<LexerException>(() => Lexer.Lex("\"\\d\""));
        }

        [Fact]
        public void Lexer_Json1()
        {
            var allTokens = Lexer.Lex("{ \"name\" : \"This is a string\" }");
            Assert.Equal(10, allTokens.Count);
            List<JsonToken> tokens = allTokens.Where(t => t.TokenType != TokenType.WhiteSpace).ToList();
            
            AssertToken("{", TokenType.OpenCurly, tokens[0]);
            AssertToken("\"name\"", TokenType.String, tokens[1]);
            AssertToken(":", TokenType.Colon, tokens[2]);
            AssertToken("\"This is a string\"", TokenType.String, tokens[3]);
            AssertToken("}", TokenType.CloseCurly, tokens[4]);
        }

        [Fact]
        public void Lexer_InsertsEndOfStreamToken()
        {
            var allTokens = Lexer.Lex("{ \"name\" : \"This is a string\" }");
            List<JsonToken> tokens = allTokens.Where(t => t.TokenType != TokenType.WhiteSpace).ToList();

            AssertToken("", TokenType.EndOfStreamToken, tokens[5]);
        }

        [Fact]
        public void Lexer_Json2()
        {
            var allTokens = Lexer.Lex("{ \"name\" : 123 }");
            Assert.Equal(10, allTokens.Count);
            List<JsonToken> tokens = allTokens.Where(t => t.TokenType != TokenType.WhiteSpace).ToList();

            AssertToken("{", TokenType.OpenCurly, tokens[0]);
            AssertToken("\"name\"", TokenType.String, tokens[1]);
            AssertToken(":", TokenType.Colon, tokens[2]);
            AssertToken("123", TokenType.Integer, tokens[3]);
            AssertToken("}", TokenType.CloseCurly, tokens[4]);
        }

        private static void AssertToken(string expectedTokenString, TokenType expectedTokenType, JsonToken actualToken)
        {
            Assert.Equal(expectedTokenString, actualToken.Token);
            Assert.Equal(expectedTokenType, actualToken.TokenType);
        }
    }
}