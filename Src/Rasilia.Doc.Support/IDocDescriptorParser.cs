namespace Rasilia.Doc.Support
{
    using System.Collections.Generic;
    using System.IO;

    public interface IDocDescriptorParser
    {
        IEnumerable<TopicInfo> Parse(Stream input);
    }
}