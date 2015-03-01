using System.Linq;
using NUnit.Framework;

namespace Rosalia.TestingSupport.Helpers
{
    using Rosalia.Core;
    using Rosalia.Core.Api;
    using Rosalia.Core.Tasks;

    public static class TaskDefinitionsMapExtensions
    {
        public static TaskDefinitionAssertions DefinitionByIndex(this RegisteredTasks definitions, int index)
        {
            var assertions = definitions.Transform((identity, dependencies) => new TaskDefinitionAssertions(identity, dependencies, definitions)).ToList();
            if (assertions.Count <= index)
            {
                return new TaskDefinitionAssertions(definitions, string.Format("Task at index {0} not found", index));
            }

            return assertions[index];
        }

        public static TaskDefinitionAssertions DefinitionByExecutable(this RegisteredTasks definitions, ITask<object> task)
        {
            return definitions
                .Transform((identity, dependencies) => new TaskDefinitionAssertions(identity, dependencies, definitions))
                .FirstOrDefault(x => x.FlowableWithDependencies.Task == task) ?? new TaskDefinitionAssertions(definitions, string.Format("Task for task {0} not found", task));
        }

        public static TaskDefinitionAssertions DefinitionById(this RegisteredTasks definitions, string id)
        {
            return definitions
                .Transform((identity, dependencies) => new TaskDefinitionAssertions(identity, dependencies, definitions))
                .FirstOrDefault(x => x.Identity.Value == id) ?? new TaskDefinitionAssertions(definitions, string.Format("Task with id {0} not found", id));
        }
    }

    public class TaskDefinitionAssertions
    {
        private readonly Identity _identity;
        private readonly FlowableWithDependencies _flowableWithDependencies;
        private readonly RegisteredTasks _definitions;
        private readonly string _notFoundMessage;

        public TaskDefinitionAssertions(Identity identity, FlowableWithDependencies flowableWithDependencies, RegisteredTasks definitions)
        {
            _identity = identity;
            _flowableWithDependencies = flowableWithDependencies;
            _definitions = definitions;
        }

        public TaskDefinitionAssertions(RegisteredTasks definitions, string notFoundMessage)
        {
            _definitions = definitions;
            _notFoundMessage = notFoundMessage;
        }

        public Identity Identity
        {
            get { return _identity; }
        }

        public FlowableWithDependencies FlowableWithDependencies
        {
            get { return _flowableWithDependencies; }
        }

        public TaskDefinitionAssertions AssertDefined()
        {
            if (Identity == null || FlowableWithDependencies == null)
            {
                Assert.Fail(_notFoundMessage);
            }

            return this;
        }

        public TaskDefinitionAssertions AssertHasNoDependencies()
        {
            AssertDefined();
            Assert.That(_flowableWithDependencies.Dependencies.Items, Is.Null.Or.Empty);

            return this;
        }

        public TaskDefinitionAssertions AssertDependsOn(string dependencyId)
        {
            AssertDefined();
            Assert.That(_flowableWithDependencies.Dependencies, Is.Not.Null.Or.Empty);
            
            var dependency = _flowableWithDependencies.Dependencies.Find(dependencyId);
            if (dependency == null)
            {
                Assert.Fail("Task [{0}] does not depend on task [{1}]", _identity.Value, dependencyId);    
            }

            return this;
        }

        public TaskDefinitionAssertions AssertExecutableIs(ITask<object> task)
        {
            AssertDefined();
            Assert.That(FlowableWithDependencies.Task, Is.EqualTo(task));

            return this;
        }

        public RegisteredTasks And()
        {
            return _definitions;
        }
    }
}