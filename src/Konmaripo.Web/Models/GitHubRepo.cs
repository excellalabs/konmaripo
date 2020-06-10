using System;

namespace Konmaripo.Web.Models
{
    public class GitHubRepo
    {
        public string Name { get; }
        public int StarCount { get; }
        public bool IsArchived { get; }
        public int ForkCount { get; }
        public int OpenIssueCount { get; }
        public DateTimeOffset CreatedDate { get; set; }

        public GitHubRepo(string name, int starCount, bool isArchived, int forkCount, int openIssues, DateTimeOffset createdDate)
        {
            Name = name;
            StarCount = starCount;
            IsArchived = isArchived;
            ForkCount = forkCount;
            OpenIssueCount = openIssues;
            CreatedDate = createdDate;
        }
    }
}