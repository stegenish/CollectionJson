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
}