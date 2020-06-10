namespace Konmaripo.Web.Models
{
    public class GitHubRepo
    {
        public string Name { get; }
        public int StarCount { get; }
        public bool IsArchived { get; }
        public int ForkCount { get; }

        public GitHubRepo(string name, int starCount, bool isArchived, int forkCount)
        {
            Name = name;
            StarCount = starCount;
            IsArchived = isArchived;
            ForkCount = forkCount;
        }
    }
}