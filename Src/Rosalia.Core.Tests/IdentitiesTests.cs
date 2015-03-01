namespace Rosalia.Core.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class IdentitiesTests
    {
        [Test]
        public void PlusSingleIdentity()
        {
            var first = new Identities(new Identity("1"), new Identity("2"));
            var second = new Identity("3");

            var result = first + second;

            Assert.That(result.Items.Length, Is.EqualTo(3));
            Assert.That(result.Items[0].Value, Is.EqualTo("1"));
            Assert.That(result.Items[1].Value, Is.EqualTo("2"));
            Assert.That(result.Items[2].Value, Is.EqualTo("3"));
        }

        [Test]
        public void PlusMultipleIdentities()
        {
            var first = new Identities(new Identity("1"), new Identity("2"));
            var second = new Identities(new Identity("3"), new Identity("4"));

            var result = first + second;

            Assert.That(result.Items.Length, Is.EqualTo(4));
            Assert.That(result.Items[0].Value, Is.EqualTo("1"));
            Assert.That(result.Items[1].Value, Is.EqualTo("2"));
            Assert.That(result.Items[2].Value, Is.EqualTo("3"));
            Assert.That(result.Items[3].Value, Is.EqualTo("4"));
        }
    }
}