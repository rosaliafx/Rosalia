namespace Rosalia.TaskLib.NuGet.Input
{
    using System.Collections.Generic;

    public class MetadataBuilder
    {
        private readonly IDictionary<string, string> _metadata;

        public MetadataBuilder(IDictionary<string, string> metadata)
        {
            _metadata = metadata;
        }

        
    }
}