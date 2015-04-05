namespace Rosalia.Core.Engine.Composing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Rosalia.Core.Api;

    public class SimpleLayersComposer : ILayersComposer
    {
        public Layer[] Compose(RegisteredTasks map, Identities filter)
        {
            var allTasks = GetAllTasks(map, filter);
            var result = new List<Layer>();

            while (allTasks.Any())
            {
                var layerTasks = allTasks.Where(task => AllDependenciesResolved(map, task.Id, result));
                var layer = new Layer(layerTasks.Select(x => new ExecutableWithIdentity(x.Task, x.Id)).ToArray());

                if (layer.IsEmpty)
                {
                    throw new Exception(GetErrorMessage(allTasks, result));
                }

                result.Add(layer);
                allTasks = allTasks.Where(item => !layer.Contains(item.Task)).ToList();
            }

            return result.ToArray();
        }

        private static string GetErrorMessage(List<ExecutableWithIdentity> allTasks, List<Layer> result)
        {
            var errorBuilder = new StringBuilder();
            errorBuilder.AppendLine(
                "Could not compose layers (probably caused by circular references). The tasks that could not be layered:");

            foreach (ExecutableWithIdentity executableWithIdentity in allTasks)
            {
                errorBuilder.AppendFormat("    [{0}]", executableWithIdentity.Id.Value);
                errorBuilder.AppendLine();
            }

            errorBuilder.AppendLine("Composed layers:");
            if (result.Count == 0)
            {
                errorBuilder.AppendLine("    none");
            }

            for (int index = 0; index < result.Count; index++)
            {
                Layer composedLayer = result[index];
                errorBuilder.AppendLine(string.Format("    {{{0}}}", index));
                foreach (ExecutableWithIdentity executableWithIdentity in composedLayer.Items)
                {
                    errorBuilder.AppendLine(string.Format("        [{0}]", executableWithIdentity.Id.Value));
                }
            }

            return errorBuilder.ToString();
        }

        private static List<ExecutableWithIdentity> GetAllTasks(RegisteredTasks map, Identities filter)
        {
            if (filter.IsEmpty)
            {
                return map.Transform((id, task) => new ExecutableWithIdentity(task.Task, id)).ToList();
            }

            List<ExecutableWithIdentity> result = new List<ExecutableWithIdentity>();
            Identities currentTasks = filter;

            while (true)
            {
                if (currentTasks.IsEmpty)
                {
                    return result;
                }

                int oldResultSize = result.Count;
                IEnumerable<ExecutableWithIdentity> iterationDependencies = map.Transform(currentTasks, (id, task) => new ExecutableWithIdentity(task.Task, id));
                foreach (ExecutableWithIdentity dependency in iterationDependencies)
                {
                    if (!result.Any(x => x.Id == dependency.Id))
                    {
                        result.Add(dependency);
                    }
                }

                int newResultSize = result.Count;

                if (newResultSize == oldResultSize)
                {
                    return result;
                }

                currentTasks = new Identities(currentTasks
                    .Items
                    .SelectMany(task => map[task].Dependencies.Items)
                    .ToArray());
            }
        }

        private bool AllDependenciesResolved(RegisteredTasks map, Identity id, IEnumerable<Layer> resolvedLayers)
        {
            return map[id].Dependencies.Items.All(dependency => DependencyResolved(dependency, resolvedLayers));
        }

        private bool DependencyResolved(Identity dependency, IEnumerable<Layer> resolvedLayers)
        {
            return resolvedLayers.Any(layer => layer.Contains(dependency));
        }
    }
}