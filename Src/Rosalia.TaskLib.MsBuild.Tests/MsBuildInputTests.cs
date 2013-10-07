namespace Rosalia.TaskLib.MsBuild.Tests
{
    using System.Linq;
    using NUnit.Framework;

    [TestFixture]
    public class MsBuildInputTests
    {
        [Test]
        public void WithSwitch_ShouldAddASwitchToCollection()
        {
            var input = new MsBuildTask()
                .WithSwitch("myswitch", "myparam");

            Assert.That(input.Switches.FirstOrDefault(x => x.Text == "myswitch" && x.Parameter == "myparam"), Is.Not.Null);
        }

        [Test]
        public void WithTargets_ShouldNotAddMultippleTargetSwitchs()
        {
            var input = new MsBuildTask()
                .WithBuildTargets("Clean", "Build")
                .WithBuildTargets("Clean", "Build");

            Assert.That(input.Switches.Count(x => x.Text == MsBuildSwitch.Target), Is.EqualTo(1));
        }

        [Test]
        public void WithTargets_ShouldJoinTargets()
        {
            var input = new MsBuildTask()
                .WithBuildTargets("Clean", "Build");

            var targetSwitch = input.Switches.FirstOrDefault(x => x.Text == MsBuildSwitch.Target);

            Assert.That(targetSwitch, Is.Not.Null);
            Assert.That(targetSwitch.Parameter, Is.EqualTo("Clean,Build"));
        }
    }
}