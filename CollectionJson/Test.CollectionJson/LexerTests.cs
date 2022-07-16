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
        public void Lexer_SingleTokenTests(string json, TokenType tokenType)
        {
            List<CJsonToken> tokens = Lexer.Lex(json);
            var actualToken = tokens.Single();
            AssertToken(json, tokenType, actualToken);
        }

        [Fact]
        public void Lexer_Json1()
        {
            var allTokens = Lexer.Lex("{ \"name\" : \"This is a string\" }");
            Assert.Equal(9, allTokens.Count);
            List<CJsonToken> tokens = allTokens.Where(t => t.TokenType != TokenType.WhiteSpace).ToList();
            
            AssertToken("{", TokenType.OpenCurly, tokens[0]);
            AssertToken("\"name\"", TokenType.String, tokens[1]);
            AssertToken(":", TokenType.Colon, tokens[2]);
            AssertToken("\"This is a string\"", TokenType.String, tokens[3]);
            AssertToken("}", TokenType.CloseCurly, tokens[4]);
        }

        [Fact]
        public void Lexer_Json2()
        {
            var allTokens = Lexer.Lex("{ \"name\" : 123 }");
            Assert.Equal(9, allTokens.Count);
            List<CJsonToken> tokens = allTokens.Where(t => t.TokenType != TokenType.WhiteSpace).ToList();

            AssertToken("{", TokenType.OpenCurly, tokens[0]);
            AssertToken("\"name\"", TokenType.String, tokens[1]);
            AssertToken(":", TokenType.Colon, tokens[2]);
            AssertToken("123", TokenType.Integer, tokens[3]);
            AssertToken("}", TokenType.CloseCurly, tokens[4]);
        }

        private static void AssertToken(string expectedTokenString, TokenType expectedTokenType, CJsonToken expectedToken)
        {
            Assert.Equal(expectedTokenString, expectedToken.Token);
            Assert.Equal(expectedTokenType, expectedToken.TokenType);
        }
    }
}