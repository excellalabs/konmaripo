namespace Konmaripo.Web.Models
{
    public class GitHubRepo
    {
        public string Name { get; }

        public GitHubRepo(string name)
        {
            Name = name;
        }
    }
}