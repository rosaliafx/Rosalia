namespace Rosalia.Runner.Console.Tests
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class OptionsParserTests
    {
        private OptionsParser _parser;

        [SetUp]
        public void Init()
        {
            _parser = new OptionsParser();
        }

        [Test]
        public void Parse_Flag_ShouldParse()
        {
            AssertValidOption(
                "/flag",
                "flag",
                value => Assert.That(value, Is.Null));
        }

        [Test]
        public void Parse_Value_ShouldParse()
        {
            AssertValidOption(
                "/foo=bar",
                "foo",
                value => Assert.That(value, Is.EqualTo("bar")));
        }

        [Test]
        public void Parse_EmptyValue_ShouldParse()
        {
            AssertValidOption(
                "/foo=", 
                "foo", 
                value => Assert.That(value, Is.EqualTo(string.Empty)));
        }

        [Test]
        public void Parse_OptionNameMismatch_ShouldNotCallAction()
        {
            AssertInvalidOption("/foo", "bar");
        }

        private void AssertValidOption(string input, string option, Action<string> optionAction)
        {
            var actionCalled = false;

            _parser.Parse(
                new[] { input },
                new OptionsConfig
                    { 
                        { 
                            new Option(option), 
                            (value, suffix) =>
                            {
                                optionAction(value);
                                actionCalled = true;
                            } 
                        }
                    });

            if (!actionCalled)
            {
                Assert.Fail("Expected option action was not called");
            }
        }

        private void AssertInvalidOption(string input, string option)
        {
            var actionCalled = false;

            _parser.Parse(
                new[] { input },
                new OptionsConfig
                    { 
                        { 
                            new Option(option), 
                            (value, suffix) =>
                            {
                                actionCalled = true;
                            } 
                        }
                    });

            if (actionCalled)
            {
                Assert.Fail("Unexpected option action was called");
            }
        }
    }
}