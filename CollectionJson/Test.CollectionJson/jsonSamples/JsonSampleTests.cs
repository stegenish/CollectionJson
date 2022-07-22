using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CollectionJson;

namespace Test.CollectionJson.jsonSamples
{
    public class JsonSampleTests
    {
        [Fact]
        public void TestSample1()
        {
            var value = Parser.Parse(Lexer.Lex(TestDataUtils.ReadFile("sample1.json")));
            Assert.Equal(CJsonType.Array, value.TopLevelType);
            var array = value.Value as List<object>;
            Assert.Equal(4, array!.Count);
            var dict = array[2] as Dictionary<string, object>;
            Assert.Equal(3L, dict!["id"]);
            Assert.Equal("nbea2@imageshack.us", dict!["email"]);
        }

        [Fact]
        public void TestSample2()
        {
            var value = Parser.Parse(Lexer.Lex(TestDataUtils.ReadFile("sample2.json")));
            Assert.Equal(CJsonType.Dictionary, value.TopLevelType);
        }
    }
}
