namespace Rosalia.Build.DocSupport.Formatting
{
    /// <summary>
    /// Converts doc source to html.
    /// </summary>
    public interface IDocFormatter
    {
        string Format(string source);
    }
}