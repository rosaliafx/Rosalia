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
            map[new Identity()] = new TaskWithBehaviors(FakeFlowable());
            map[new Identity()] = new TaskWithBehaviors(FakeFlowable());

            var result = ComposeWithAllTasksAsFilter(map).ToList();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result[0].Items.Count(), Is.EqualTo(2));
        }

        [Test]
        public void Compose_TaskWithDependency_ShouldGoToDifferentLayer()
        {
            var map = new TaskMap();
            var idPrimary = new Identity();
            var idSecondary = new Identity();

            map[idPrimary] = new TaskWithBehaviors(FakeFlowable());
            map[idSecondary] = new TaskWithBehaviors(FakeFlowable(), new DependsOnBehavior(idPrimary));

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
            var idPrimary = new Identity();
            var idSecondary1 = new Identity();
            var idSecondary2 = new Identity();

            map[idPrimary] = new TaskWithBehaviors(FakeFlowable());
            map[idSecondary1] = new TaskWithBehaviors(FakeFlowable(), new DependsOnBehavior(idPrimary));
            map[idSecondary2] = new TaskWithBehaviors(FakeFlowable(), new DependsOnBehavior(idPrimary));

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
                var idPrimary = new Identity();
                var idSecondary = new Identity();

                map[idPrimary] = new TaskWithBehaviors(FakeFlowable(), new DependsOnBehavior(idSecondary));
                map[idSecondary] = new TaskWithBehaviors(FakeFlowable(), new DependsOnBehavior(idPrimary));

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