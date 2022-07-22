namespace Test.CollectionJson;

public class ParsedJson
{
    public CJsonType TopLevelType { get; }
    public object? Value { get; }

    public ParsedJson(CJsonType topLevelType, object? value)
    {
        TopLevelType = topLevelType;
        Value = value;
    }
}