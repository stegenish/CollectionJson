namespace CollectionJson;

public class ParsedJson
{
    public ValueType TopLevelType { get; }
    public object? Value { get; }

    public ParsedJson(ValueType topLevelType, object? value)
    {
        TopLevelType = topLevelType;
        Value = value;
    }
}