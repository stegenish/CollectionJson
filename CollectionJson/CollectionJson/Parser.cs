using static CollectionJson.TokenType;

namespace CollectionJson;

public class Parser
{
    public static ParsedJson Parse(List<JsonToken> tokens)
    {
        using var tokenStream = tokens.Where(t => t.TokenType != WhiteSpace).GetEnumerator();
        tokenStream.MoveNext();
        var value = ParseValue(tokenStream);
        return value switch
        {
            Dictionary<string, object> => new ParsedJson(ValueType.Dictionary, value),
            List<object> => new ParsedJson(ValueType.Array, value),
            long => new ParsedJson(ValueType.Integer, value),
            decimal => new ParsedJson(ValueType.Decimal, value),
            string => new ParsedJson(ValueType.String, value),
        };
    }

    private static object ParseValue(IEnumerator<JsonToken> tokenStream)
    {
        return tokenStream.Current.TokenType switch
        {
            TokenType.String => Consume(tokenStream, TokenType.String).AsString(),
            TokenType.Integer => Consume(tokenStream, TokenType.Integer).AsLong(),
            TokenType.Decimal => Consume(tokenStream, TokenType.Decimal).AsDecimal(),
            TokenType.OpenCurly => ParseDictionary(tokenStream),
            TokenType.OpenSquare => ParseArray(tokenStream)
        };
    }

    private static Dictionary<string, object> ParseDictionary(IEnumerator<JsonToken> tokenStream)
    {
        Consume(tokenStream, OpenCurly);
        var dictionary = new Dictionary<string, object>();
        bool complete = tokenStream.Current.TokenType == CloseCurly;
        while (!complete)
        {
            var v = ParseKeyValue(tokenStream);
            dictionary.Add(v.key, v.valueStr);

            complete = tokenStream.Current.TokenType == CloseCurly;
            
            if (!complete)
            {
                Consume(tokenStream, Comma);
            }
        }

        Consume(tokenStream, CloseCurly);

        return dictionary;
    }

    private static List<object> ParseArray(IEnumerator<JsonToken> tokenStream)
    {
        Consume(tokenStream, OpenSquare);
        var array = new List<object>();
        var complete = tokenStream.Current.TokenType == CloseSquare;
        while (!complete)
        {
            array.Add(ParseValue(tokenStream));

            complete = tokenStream.Current.TokenType == CloseSquare;
            if (!complete)
            {
                Consume(tokenStream, Comma);
            }
        }

        Consume(tokenStream, CloseSquare);

        return array;
    }

    private static (string key, object valueStr) ParseKeyValue(IEnumerator<JsonToken> tokenStream)
    {
        var key = Consume(tokenStream, TokenType.String).AsString();
        Consume(tokenStream, Colon);
        var valueStr = ParseValue(tokenStream);
        var v = (key, valueStr);
        return v;
    }

    private static JsonToken Consume(IEnumerator<JsonToken> tokenStream, TokenType tokenType)
    {
        var current = tokenStream.Current;
        if (current.TokenType == tokenType && tokenStream.MoveNext())
        {
            return current;
        }

        throw new ParseException($"Error parsing token {tokenStream.Current.ParseErrorMsg()}, expected {tokenType} ");
    }
}