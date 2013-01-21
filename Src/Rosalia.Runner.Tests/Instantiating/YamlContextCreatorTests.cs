namespace Rosalia.Runner.Tests.Instantiating
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using NUnit.Framework;
    using Rosalia.Runner.Instantiating;
    using Rosalia.Runner.Lookup;

    [TestFixture]
    public class YamlContextCreatorTests
    {
        [Test]
        public void CreateContext_SimpleContext_ShouldFillProperties()
        {
            const string yaml = 
@"---
FirstName: Marilyn
LastName: Manson
";
            var context = CreateContext<SimpleContext>(yaml);

            Assert.That(context, Is.Not.Null);
            Assert.That(context.FirstName, Is.EqualTo("Marilyn"));
            Assert.That(context.LastName, Is.EqualTo("Manson"));
        }

        [Test]
        public void CreateContext_SimpleContextWithArray_ShouldFillProperties()
        {
            const string yaml =
@"---
AlbumTitles:
    - Portrait of an American Family
    - Smells Like Children
    - Antichrist Superstar
    - Mechanical Animals
    - Holy Wood (In the Shadow of the Valley of Death)
    - The Golden Age of Grotesque
    - Eat Me, Drink Me
    - The High End of Low
    - Born Villain
";
            var context = CreateContext<SimpleContext>(yaml);

            Assert.That(context, Is.Not.Null);
            Assert.That(context.AlbumTitles, Is.Not.Null);
            Assert.That(context.AlbumTitles.Length, Is.EqualTo(9));
        }

        [Test]
        public void CreateContext_SimpleContextWithList_ShouldFillProperties()
        {
            const string yaml =
@"---
Albums:
    - Title: Portrait of an American Family
      Year: 1994
    - Title: Smells Like Children
      Year: 1995
";
            var context = CreateContext<SimpleContext>(yaml);

            Assert.That(context, Is.Not.Null);
            Assert.That(context.Albums, Is.Not.Null);
            Assert.That(context.Albums.Length, Is.EqualTo(2));
            Assert.That(context.Albums[0].Title, Is.EqualTo("Portrait of an American Family"));
            Assert.That(context.Albums[0].Year, Is.EqualTo(1994));
            Assert.That(context.Albums[1].Title, Is.EqualTo("Smells Like Children"));
            Assert.That(context.Albums[1].Year, Is.EqualTo(1995));
        }

        private T CreateContext<T>(string yaml)
        {
            var workflowInfo = new WorkflowInfo
            {
                ContextType = typeof(T)
            };
            var contextCreator = new YamlContextCreator(CreateProvider(yaml));
            var context = contextCreator.CreateContext(workflowInfo);

            return (T)context;
        }

        private Func<Stream> CreateProvider(string text)
        {
            return () => new MemoryStream(Encoding.Default.GetBytes(text));
        }

        private class SimpleContext
        {
            public string FirstName { get; set; }

            public string LastName { get; set; }

            public string[] AlbumTitles { get; set; }

            public Album[] Albums { get; set; }
        }

        private class Album
        {
            public string Title { get; set; }

            public int Year { get; set; }
        }
    }
}