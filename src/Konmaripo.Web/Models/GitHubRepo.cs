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
        public DateTimeOffset CreatedDate { get; }
        public DateTimeOffset UpdatedDate { get; }
        public long Id { get; }
        public string Description { get; }
        public bool IsPrivate { get; }
        public int WatcherCount { get; }

        public GitHubRepo(long repoId, string name, int starCount, bool isArchived, int forkCount, int openIssues, DateTimeOffset createdDate, DateTimeOffset updatedDate, string description, bool isPrivate, int watcherCount)
        {
            Name = name;
            StarCount = starCount;
            IsArchived = isArchived;
            ForkCount = forkCount;
            OpenIssueCount = openIssues;
            CreatedDate = createdDate;
            UpdatedDate = updatedDate;
            Id = repoId;
            Description = description;
            IsPrivate = isPrivate;
            WatcherCount = watcherCount;
        }
    }
}