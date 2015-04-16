namespace Rosalia.Core.Tests.Api
{
    using NUnit.Framework;
    using Rosalia.Core.Api;
    using Rosalia.TestingSupport.Executables;
    using Rosalia.TestingSupport.Helpers;

    [TestFixture]
    public class AbstractTaskRegistry_Execution
    {
        [Test]
        public void SingleSubflowTask()
        {
            var result = QuickSubflow<string>
                .Define(s => s.Task("", Task.Const("foo")))
                .Execute();

            result.AssertDataIs("foo");
        }

        [Test]
        public void MultipleLinqSubflowTask()
        {
            var result = QuickSubflow<string>
                .Define(s =>
                {
                    var fooTask = s.Task("1", Task.Const("foo"));
                    var barTask = s.Task("2", Task.Const("bar"));
                    var bazTask = s.Task("3", Task.Const("baz"));

                    return s.Task(
                        "4",
                        from foo in fooTask
                        from bar in barTask
                        from baz in bazTask
                        select foo + " " + bar + " " + baz);
                })
                .Execute();

            result.AssertDataIs("foo bar baz");
        }

        [Test]
        public void SimpleTaskWithPrecondition()
        {
            var result = QuickSubflow<string>
                .Define(s => s.Task("", Task.Const("foo").WithPrecondition(() => false, "bar")))
                .Execute();

            result.AssertDataIs("bar");
        }

        [Test]
        public void DependentTaskWithPrecondition()
        {
            var result = QuickSubflow<string>
                .Define(s =>
                {
                    var fooTask = s.Task("", Task.Const("foo"));
                    var barTask = s.Task("", Task.Const("bar"));
                    var bazTask = s.Task(
                        "BazTask",
                        context =>
                        {
                            context.Log.Info("Calculating baz!");
                            return "baz".AsTaskResult();
                        });

                    return s.Task(@"
                        ======================
                        = [MainTask]
                        ======================",
                        from foo in fooTask
                        from bar in barTask
                        from baz in bazTask
                        select Task
                            .Const(foo + " " + bar + " " + baz)
                            .WithPrecondition(foo == null, (foo + baz).ToUpper()));
                })
                .Execute();

            result.AssertDataIs("FOOBAZ");
        }

        [Test]
        public void SimpleTaskWithRecovery()
        {
            var result = QuickSubflow<string>
                .Define(s => s.Task("", Task
                    .Failure<string>()
                    .RecoverWith(() => "bar")))
                .Execute();

            result.AssertDataIs("bar");
        }

        [Test]
        public void DependentTaskWithRecovery()
        {
            var result = QuickSubflow<string>
                .Define(s =>
                {
                    var fooTask = s.Task("", Task.Const("foo"));
                    var barTask = s.Task("", Task.Const("bar"));
                    var bazTask = s.Task("", Task.Const("baz"));

                    return s.Task(
                        string.Empty,
                        from foo in fooTask
                        from bar in barTask
                        from baz in bazTask
                        select Task
                            .Failure<string>()
                            .RecoverWith("FOO"));
                })
                .Execute();

            result.AssertDataIs("FOO");
        }
    }
}