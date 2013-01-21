namespace Rosalia.Runner.Instantiating
{
    using System;
    using System.IO;
    using System.Yaml.Serialization;
    using Rosalia.Runner.Lookup;

    public class YamlContextCreator : IContextCreator
    {
        private readonly Func<Stream> _streamProvider;
        private readonly YamlSerializer _serializer = new YamlSerializer();

        public YamlContextCreator(Func<Stream> streamProvider)
        {
            _streamProvider = streamProvider;
        }

        public object CreateContext(WorkflowInfo workflowInfo)
        {
            var result = _serializer.Deserialize(
                _streamProvider.Invoke(),
                workflowInfo.ContextType);

            return result[0];
        }
    }
}