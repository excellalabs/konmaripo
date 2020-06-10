namespace Konmaripo.Web.Models
{
    public class GitHubRepo
    {
        public string Name { get; }
        public int StarCount { get; }

        public GitHubRepo(string name, int starCount)
        {
            Name = name;
            StarCount = starCount;
        }
    }
}