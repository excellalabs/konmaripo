namespace Konmaripo.Web.Services
{
    /// <summary>
    /// POCO to represent a strongly typed value so we know where we're using it.
    /// The repository path represents the path on disk to the created repository.
    /// </summary>
    public class RepositoryPath
    {
        public string Value { get; }

        public RepositoryPath(string pathValue)
        {
            Value = pathValue;
        }
    }
}