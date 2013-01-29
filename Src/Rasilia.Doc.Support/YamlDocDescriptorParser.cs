namespace Rasilia.Doc.Support
{
    using System.Collections.Generic;
    using System.IO;
    using System.Yaml.Serialization;

    public class YamlDocDescriptorParser : IDocDescriptorParser
    {
        public IEnumerable<TopicInfo> Parse(Stream input)
        {
            var descriptor = new YamlSerializer().Deserialize(input, typeof(DocDescriptor))[0];
            return ((DocDescriptor)descriptor).Topics;
        }
    }
}