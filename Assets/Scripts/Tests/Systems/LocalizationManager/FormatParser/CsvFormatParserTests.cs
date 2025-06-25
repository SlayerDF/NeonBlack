using System;
using NeonBlack.Systems.LocalizationManager;
using NUnit.Framework;

namespace NeonBlack.Tests.Systems.LocalizationManager.FormatParser
{
    public class CsvFormatParserTests
    {
        private CsvFormatParser parser;

        [SetUp]
        public void Setup()
        {
            parser = new CsvFormatParser();
        }

        [Test]
        public void ParsesSingleLineCorrectly()
        {
            const string input = "hello;world\n";
            var result = parser.Parse(input);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("world", result["hello"]);
        }

        [Test]
        public void ParsesMultipleLines()
        {
            const string input = "a;1\nb;2\nc;3\n";
            var result = parser.Parse(input);

            Assert.AreEqual("1", result["a"]);
            Assert.AreEqual("2", result["b"]);
            Assert.AreEqual("3", result["c"]);
        }

        [Test]
        public void HandlesFinalLineWithoutNewline()
        {
            const string input = "foo;bar\nbaz;qux";
            var result = parser.Parse(input);

            Assert.AreEqual("bar", result["foo"]);
            Assert.AreEqual("qux", result["baz"]);
        }

        [Test]
        public void IgnoresEmptyLines()
        {
            const string input = "a;1\n\nb;2\n";
            var result = parser.Parse(input);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("1", result["a"]);
            Assert.AreEqual("2", result["b"]);
        }

        [Test]
        public void HandlesWindowsNewlines()
        {
            const string input = "x;10\r\ny;20\r\n";
            var result = parser.Parse(input);

            Assert.AreEqual("10", result["x"]);
            Assert.AreEqual("20", result["y"]);
        }

        [Test]
        public void HandlesSemicolonInsideValue()
        {
            const string input = "message;Error;something went wrong\n";
            var result = parser.Parse(input);

            Assert.AreEqual("Error;something went wrong", result["message"]);
        }

        [Test]
        public void ThrowsErrorIfDuplicateKey()
        {
            const string input = "a;1\na;2\n";

            var error = Assert.Throws<ArgumentException>(() => parser.Parse(input));
            StringAssert.Contains("Key: a", error.Message);
        }

        [Test]
        public void ThrowsErrorIfSemicolonIsMissing()
        {
            const string input = "valid;ok\nbroken_line\nnext;yes\n";

            var error = Assert.Throws<FormatException>(() => parser.Parse(input));
            StringAssert.Contains("broken_line", error.Message);
        }
    }
}
