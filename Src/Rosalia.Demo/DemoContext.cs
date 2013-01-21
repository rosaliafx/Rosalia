namespace Rosalia.Demo
{
    public class DemoContext
    {
        public DemoContext()
        {
            FirstName = "Oleg";
            LastName = "Ivanov";
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string VersionNumber { get; set; }
    }
}