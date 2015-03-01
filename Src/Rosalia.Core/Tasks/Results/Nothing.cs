namespace Rosalia.Core.Tasks.Results
{
    public class Nothing
    {
        public static Nothing Value
        {
            get
            {
                return new Nothing();
            }
        }

        private Nothing()
        {
        }
    }
}