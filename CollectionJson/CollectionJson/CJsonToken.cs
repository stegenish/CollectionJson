using System.Globalization;

namespace CollectionJson;

public class CJsonToken
{
    public string Token { get; }
    public TokenType TokenType { get; }
    public CJsonToken(string token, TokenType tokenType)
    {
        Token = token;
        TokenType = tokenType;
    }

    public string AsString()
    {
        return Token.Substring(1, Token.Length - 2);
    }

    public string ParseErrorMsg()
    {
        return $"{Token}, {TokenType}";
    }

    public long AsLong()
    {
        return long.Parse(Token);
    }

    public Decimal AsDecimal()
    {
        return Decimal.Parse(Token, NumberStyles.Number, CultureInfo.InvariantCulture);
    }
}