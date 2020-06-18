using System;
using Bogus;
using Octokit;

namespace Konmaripo.Web.Tests.Unit.Helpers
{
    public class RepositoryBuilder
    {
        const string DummyString = "";
        const long DummyLong = 0;
        const int DummyInt = 0;
        private readonly DateTimeOffset _dummyDateTimeOffset;

        private string _repositoryName = "";
        private int _starCount = 0;
        private bool _isArchived = false;
        private int _forkCount = 0;
        private int _openIssueCount = 0;
        private DateTimeOffset _createdDate = DateTimeOffset.Now;
        private DateTimeOffset _updatedDate = DateTimeOffset.Now;
        private long _repoId = 0;
        private string _description = "";
        private bool _isPrivate = false;
        private DateTimeOffset _pushedDate = DateTimeOffset.Now;
        private string _repoUrl = "";

        private Faker<Repository> _faker = new Faker<Repository>();

        public RepositoryBuilder()
        {
            _dummyDateTimeOffset = DateTimeOffset.Now;
        }

        public RepositoryBuilder WithName(string repoName)
        {
            _faker.RuleFor(x => x.Name, repoName);

            return this;
        }

        public Repository Build()
        {
            return _faker.Generate();
        }

        public RepositoryBuilder WithStarCount(int starCount)
        {
            _faker.RuleFor(x => x.StargazersCount, starCount);
            return this;
        }

        public RepositoryBuilder WithArchivedOf(bool archived)
        {
            _faker.RuleFor(x => x.Archived, archived);
            return this;
        }

        public RepositoryBuilder WithForkCount(int forkCount)
        {
            _faker.RuleFor(x => x.ForksCount, forkCount);
            return this;
        }

        public RepositoryBuilder WithOpenIssues(int openIssueCount)
        {
            _faker.RuleFor(x => x.OpenIssuesCount, openIssueCount);
            return this;
        }

        public RepositoryBuilder WithCreatedDate(DateTimeOffset createdDate)
        {
            _faker.RuleFor(x => x.CreatedAt, createdDate);
            return this;
        }

        public RepositoryBuilder WithUpdatedDate(DateTimeOffset updatedDate)
        {
            _faker.RuleFor(x => x.UpdatedAt, updatedDate);
            return this;
        }

        public RepositoryBuilder WithId(long repoId)
        {
            _faker.RuleFor(x => x.Id, repoId);
            return this;
        }

        public RepositoryBuilder WithDescription(string desc)
        {
            _faker.RuleFor(x => x.Description, desc);
            return this;
        }

        public RepositoryBuilder WithIsPrivateOf(bool isPrivate)
        {
            _faker.RuleFor(x => x.Private, isPrivate);
            return this;
        }

        public RepositoryBuilder WithPushedDate(DateTimeOffset pushedDate)
        {
            _faker.RuleFor(x => x.PushedAt, pushedDate);
            return this;
        }

        public RepositoryBuilder WithUrlOf(string url)
        {
            _faker.RuleFor(x => x.HtmlUrl, url);
            return this;
        }
    }

}
