namespace Rosalia.Core.FileSystem
{
    public class ExtensionInfo
    {
        private readonly string _canonicalExtension;

        public ExtensionInfo(string extension)
        {
            _canonicalExtension = GetCanonicalForm(extension);
        }

        public string CanonicalExtension
        {
            get { return _canonicalExtension; }
        }

        public override int GetHashCode()
        {
            return _canonicalExtension != null ? _canonicalExtension.GetHashCode() : 0;
        }

        public override bool Equals(object obj)
        {
            var extensionInfo = obj as ExtensionInfo;
            if (extensionInfo != null)
            {
                return extensionInfo.CanonicalExtension == CanonicalExtension;
            }

            return base.Equals(obj);
        }

        public bool Is(string extension)
        {
            return GetCanonicalForm(extension) == CanonicalExtension;
        }

        private string GetCanonicalForm(string extension)
        {
            if (extension.StartsWith("."))
            {
                extension = extension.Substring(1);
            }

            return extension.ToUpper();
        }
    }
}