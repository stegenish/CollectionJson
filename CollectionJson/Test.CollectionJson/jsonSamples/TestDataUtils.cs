namespace Test.CollectionJson.jsonSamples;

public class TestDataUtils
{
    public static string ReadFile(string fileName)
    {
        var stream = typeof(TestDataUtils)
            .Assembly
            .GetManifestResourceStream($"Test.CollectionJson.jsonSamples.{fileName}");
        if (stream == null) throw new FileNotFoundException($"{fileName} not found");

        return new StreamReader(stream).ReadToEnd();
    }
}