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

            var repo = new Repository(DummyString, DummyString, DummyString, DummyString, DummyString, DummyString,
                DummyString, DummyLong, DummyString, owner, _repositoryName, DummyString, false, DummyString,
                DummyString, DummyString, false, false, _forkCount, _starCount, DummyString, DummyInt, _dummyDateTimeOffset,
                _dummyDateTimeOffset, _dummyDateTimeOffset, repositoryPermissions,
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
    }

}
