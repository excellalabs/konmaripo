using System;
using System.Collections.Generic;
using System.Text;
using Octokit;

namespace Konmaripo.Web.Tests.Unit.Helpers
{
    public class RepositoryBuilder
    {
        private string _repositoryName = "";
        const string DummyString = "";
        const long DummyLong = 0;
        const int DummyInt = 0;
        private readonly DateTimeOffset DummyDateTimeOffset;

        public RepositoryBuilder()
        {
            DummyDateTimeOffset = DateTimeOffset.Now;
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
                DummyString, DummyString, false, false, DummyInt, DummyInt, DummyString, DummyInt, DummyDateTimeOffset,
                DummyDateTimeOffset, DummyDateTimeOffset, repositoryPermissions,
                dummyInternalRepository, dummyInternalRepository,
                licenseMetadata,
                false, false, false, false, 0, 0, false, false, false, false);

            return repo;
        }

    }

}
