using System;
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

        public RepositoryBuilder()
        {
            _dummyDateTimeOffset = DateTimeOffset.Now;
        }

        public RepositoryBuilder WithName(string repoName)
        {
            _repositoryName = repoName;

            return this;
        }

        public Repository Build()
        {
            var dummyInternalRepository = new Repository(1);
            var licenseMetadata = new LicenseMetadata(DummyString, DummyString, DummyString, DummyString, DummyString, false);
            var repositoryPermissions = new RepositoryPermissions(false, false, false);
            var owner = new User();

            var repo = new Repository(_repoUrl, DummyString, DummyString, DummyString, DummyString, DummyString,
                DummyString, _repoId, DummyString, owner, _repositoryName, DummyString, false, _description,
                DummyString, DummyString, _isPrivate, false, _forkCount, _starCount, DummyString, _openIssueCount, _pushedDate,
                _createdDate, _updatedDate, repositoryPermissions,
                dummyInternalRepository, dummyInternalRepository,
                licenseMetadata,
                false, false, false, false, 0, 0, false, false, false, _isArchived);

            return repo;
        }

        public RepositoryBuilder WithStarCount(int starCount)
        {
            _starCount = starCount;
            return this;
        }

        public RepositoryBuilder WithArchivedOf(bool archived)
        {
            _isArchived = archived;
            return this;
        }

        public RepositoryBuilder WithForkCount(int forkCount)
        {
            _forkCount = forkCount;
            return this;
        }

        public RepositoryBuilder WithOpenIssues(int openIssueCount)
        {
            _openIssueCount = openIssueCount;
            return this;
        }

        public RepositoryBuilder WithCreatedDate(DateTimeOffset createdDate)
        {
            _createdDate = createdDate;
            return this;
        }

        public RepositoryBuilder WithUpdatedDate(DateTimeOffset updatedDate)
        {
            _updatedDate = updatedDate;
            return this;
        }

        public RepositoryBuilder WithId(long repoId)
        {
            _repoId = repoId;
            return this;
        }

        public RepositoryBuilder WithDescription(string desc)
        {
            _description = desc;
            return this;
        }

        public RepositoryBuilder WithIsPrivateOf(bool isPrivate)
        {
            _isPrivate = isPrivate;
            return this;
        }

        public RepositoryBuilder WithPushedDate(DateTimeOffset pushedDate)
        {
            _pushedDate = pushedDate;
            return this;
        }

        public RepositoryBuilder WithUrlOf(string url)
        {
            _repoUrl = url;
            return this;
        }
    }

}
