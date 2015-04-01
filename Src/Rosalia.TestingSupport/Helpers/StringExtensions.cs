namespace Rosalia.TestingSupport.Helpers
{
    using System.Text.RegularExpressions;

    public static class StringExtensions
    {
        public static string NormalizeLineEnding(this string input)
        {
            return Regex.Replace(input, @"\r\n|\n\r|\n|\r", "\r\n");
        }
    }
}