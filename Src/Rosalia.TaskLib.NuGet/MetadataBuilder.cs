namespace Rosalia.TaskLib.NuGet
{
    using System.Collections.Generic;
    using System.Globalization;

    public class MetadataBuilder
    {
        private readonly IDictionary<string, string> _metadata;

        public MetadataBuilder(IDictionary<string, string> metadata)
        {
            _metadata = metadata;
        }

        
    }
}