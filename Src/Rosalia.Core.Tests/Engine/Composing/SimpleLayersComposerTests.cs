namespace Rosalia.Core.Tests.Engine.Composing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;
    using Rosalia.Core.Api;
    using Rosalia.Core.Api.Behaviors;
    using Rosalia.Core.Engine.Composing;
    using Rosalia.Core.Tasks;

    [TestFixture]
    public class SimpleLayersComposerTests
    {
        [Test]
        public void Compose_EmptyDefinitions_ShouldReturnEmptyList()
        {
            var result = ComposeWithAllTasksAsFilter(new TaskMap());

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(0));
        }

        [Test]
        public void Compose_IndependentTasks_ShouldGoToTheSameLayer()
        {
            var map = new TaskMap();

            map.Register("1", FakeFlowable(), new ITaskBehavior[0]);
            map.Register("2", FakeFlowable(), new ITaskBehavior[0]);

//            map[new Identity()] = new TaskWithBehaviors(FakeFlowable());
//            map[new Identity()] = new TaskWithBehaviors(FakeFlowable());

            var result = ComposeWithAllTasksAsFilter(map).ToList();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result[0].Items.Count(), Is.EqualTo(2));
        }

        [Test]
        public void Compose_TaskWithDependency_ShouldGoToDifferentLayer()
        {
            var map = new TaskMap();
            var idPrimary = new Identity("primary");
            var idSecondary = new Identity("secondary");

            map.Register("primary", FakeFlowable(), new ITaskBehavior[0]);
            map.Register("secondary", FakeFlowable(), new ITaskBehavior[] { new DependsOnBehavior(idPrimary) });

            var result = ComposeWithAllTasksAsFilter(map).ToList();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));

            Assert.That(result[0].Items.Count(), Is.EqualTo(1));
            Assert.That(result[1].Items.Count(), Is.EqualTo(1));

            Assert.That(result[0].Items[0].Id, Is.EqualTo(idPrimary));
            Assert.That(result[1].Items[0].Id, Is.EqualTo(idSecondary));
        }

        [Test]
        public void Compose_WithFilter_ShouldPassOnlyAllowedTasks()
        {
            var map = new TaskMap();
            var idPrimary = new Identity("primary");
            var idSecondary2 = new Identity("secondary2");

            map.Register("primary", FakeFlowable(), new ITaskBehavior[0]);
            map.Register("secondary1", FakeFlowable(), new ITaskBehavior[] { new DependsOnBehavior(idPrimary) });
            map.Register("secondary2", FakeFlowable(), new ITaskBehavior[] { new DependsOnBehavior(idPrimary) });

            var result = ComposeWithFilter(map, Identities.Empty + idSecondary2).ToList();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));

            Assert.That(result[0].Items.Count(), Is.EqualTo(1));
            Assert.That(result[1].Items.Count(), Is.EqualTo(1));

            Assert.That(result[0].Items[0].Id, Is.EqualTo(idPrimary));
            Assert.That(result[1].Items[0].Id, Is.EqualTo(idSecondary2));
        }

        [Test]
        public void Compose_CircularReference_ShouldThrowException()
        {
            Assert.Throws<Exception>(() =>
            {
                var map = new TaskMap();

                var idPrimary = new Identity("primary");
                var idSecondary = new Identity("secondary");

                map.Register("primary", FakeFlowable(), new ITaskBehavior[] { new DependsOnBehavior(idSecondary) });
                map.Register("secondary", FakeFlowable(), new ITaskBehavior[] { new DependsOnBehavior(idPrimary) });

//                map[idPrimary] = new TaskWithBehaviors(FakeFlowable(), new DependsOnBehavior(idSecondary));
//                map[idSecondary] = new TaskWithBehaviors(FakeFlowable(), new DependsOnBehavior(idPrimary));

                ComposeWithAllTasksAsFilter(map);
            });
        }

        private static FuncTask<object> FakeFlowable()
        {
            return new FuncTask<object>(null);
        }

        private IEnumerable<Layer> ComposeWithFilter(TaskMap map, Identities filter)
        {
            return new SimpleLayersComposer().Compose(new RegisteredTasks(map.Map, null, Identities.Empty), filter);
        }

        private IEnumerable<Layer> ComposeWithAllTasksAsFilter(TaskMap map)
        {
            var startupTaskIds = new Identities(map.Map.Keys.ToArray());
            return new SimpleLayersComposer().Compose(new RegisteredTasks(map.Map, null, Identities.Empty), startupTaskIds);
        }
    }
}