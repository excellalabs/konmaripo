using System;
using System.Collections.Generic;
using Functional.Maybe;

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
        public Maybe<DateTimeOffset> PushedDate { get; }
        public string RepoUrl { get; }
        public int Subscribers { get; }
        public IReadOnlyList<string> Topics { get; }

        public GitHubRepo(long repoId, string name, int starCount, bool isArchived, int forkCount, int openIssues, DateTimeOffset createdDate, DateTimeOffset updatedDate, string description, bool isPrivate, DateTimeOffset? pushedDate, string url, int subscribers, IReadOnlyList<string> topics)
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
            PushedDate = pushedDate.ToMaybe();
            RepoUrl = url;
            Subscribers = subscribers;
            Topics = topics;
        }
    }
}