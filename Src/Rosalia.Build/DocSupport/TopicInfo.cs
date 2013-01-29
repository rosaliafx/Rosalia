namespace Rosalia.Build.DocSupport
{
    public class TopicInfo
    {
        private string _place;

        public string Path { get; set; }

        public string Place
        {
            get { return _place; }

            set
            {
                _place = value;
                PlaceParts = _place.Split('/');
            }
        }

        public string Title { get; set; }

        public string[] PlaceParts { get; private set; }

        public int Level
        {
            get { return PlaceParts.Length; }
        }
    }
}