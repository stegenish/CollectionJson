using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using CollectionJson;
using ValueType = CollectionJson.ValueType;

namespace Test.CollectionJson
{
    public class ParserTests
    {
        [Fact]
        public void Parse_EmptyDictionary()
        {
            ParsedJson result = Parser.Parse(Lexer.Lex("{}"));

            Assert.Equal(ValueType.Dictionary, result.TopLevelType);
            Assert.IsType<Dictionary<string, object>>(result.Value);
        }


        [Fact]
        public void Parse_EmptyArray()
        {
            ParsedJson result = Parser.Parse(Lexer.Lex("[]"));

            Assert.Equal(ValueType.Array, result.TopLevelType);
            Assert.IsType<List<object>>(result.Value);
        }

        [Fact]
        public void Parse_ArrayWithEmptyArray()
        {
            ParsedJson result = Parser.Parse(Lexer.Lex("[[]]"));

            Assert.Equal(ValueType.Array, result.TopLevelType);
            var resultValue = result.Value as List<object>;
            Assert.IsType<List<object>>(resultValue![0]);
        }

        [Fact]
        public void Parse_ArrayWithElements()
        {
            ParsedJson result = Parser.Parse(Lexer.Lex(@"[1, ""2""]"));

            Assert.Equal(ValueType.Array, result.TopLevelType);
            var resultValue = result.Value as List<object>;
            Assert.Equal("2", resultValue![1]);
        }

        [Fact]
        public void Parse_ArrayWithMissingComma_ThrowsException()
        {
            Assert.Throws<ParseException>(() => Parser.Parse(Lexer.Lex(@"[1 ""2""]")));
        }

        [Fact]
        public void Parse_JsonProperties_DictionaryWithValues()
        {
            ParsedJson result = Parser.Parse(Lexer.Lex(@"{""test"" : ""text""}"));

            Assert.Equal(ValueType.Dictionary, result.TopLevelType);
            var dictionary = result.Value as Dictionary<string, object>;
            Debug.Assert(dictionary != null, nameof(dictionary) + " != null");
            var value = dictionary["test"];
            Assert.IsType<string>(value);
            Assert.Equal("text", value);
        }

        [Fact]
        public void Parse_IntegerProperty_ValueIsLong()
        {
            ParsedJson result = Parser.Parse(Lexer.Lex(@"{""test"" : 123}"));

            var dictionary = result.Value as Dictionary<string, object>;
            
            var value = (long)dictionary!["test"];
            
            Assert.Equal(123, value);
        }

        [Fact]
        public void Parse_DecimalProperty_ValueIsDecimal()
        {
            ParsedJson result = Parser.Parse(Lexer.Lex(@"{""test"" : 123.123}"));

            var dictionary = result.Value as Dictionary<string, object>;

            var value = (decimal)dictionary!["test"];

            Assert.Equal(123.123m, value);
        }

        [Fact]
        public void Parse_DictionaryProperty_ValueDictionary()
        {
            ParsedJson result = Parser.Parse(Lexer.Lex(@"{""test"" : {}}"));

            var dictionary = result.Value as Dictionary<string, object>;

            var value = dictionary!["test"] as Dictionary<string, object>;

            Assert.NotNull(value);
        }

        [Fact]
        public void Parse_MultipleProperties()
        {
            ParsedJson result = Parser.Parse(Lexer.Lex(@"{""test"" : ""text"", ""test2"" : ""text2""}"));

            Assert.Equal(ValueType.Dictionary, result.TopLevelType);
            var dictionary = result.Value as Dictionary<string, object>;
            Assert.Equal(2, dictionary!.Count);
            var value = dictionary!["test2"];
            Assert.IsType<string>(value);
            Assert.Equal("text2", value);
        }

        [Fact]
        public void Parse_MissingComma_ThrowsException()
        {
            Assert.Throws<ParseException>(() => Parser.Parse(Lexer.Lex(@"{""test"" : ""text"" ""test2"" : ""text2""}")));
        }
    }
}
