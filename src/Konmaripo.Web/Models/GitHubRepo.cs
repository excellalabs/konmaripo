namespace Konmaripo.Web.Models
{
    public class GitHubRepo
    {
        public string Name { get; }
        public int StarCount { get; }
        public bool IsArchived { get; set; }

        public GitHubRepo(string name, int starCount, bool isArchived)
        {
            Name = name;
            StarCount = starCount;
            IsArchived = isArchived;
        }
    }
}