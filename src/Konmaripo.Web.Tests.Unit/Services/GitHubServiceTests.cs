using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Konmaripo.Web.Services;
using Moq;
using Octokit;
using Xunit;

namespace Konmaripo.Web.Tests.Unit.Services
{

    public class GitHubServiceTests
    {
        public class Ctor
        {
            [Fact]
            public void NullGitHubClient_ThrowsException()
            {
                Action sut = () => new GitHubService(githubClient: null);

                sut.Should().Throw<ArgumentNullException>()
                    .And.ParamName.Should().Be("githubClient");
            }
        }

        public class GetRepositoriesForOrganization
        {
            [Fact]
            public async Task ReturnsTheRepositoryNamesFromTheGithubClient()
            {
                var repositoryNames = new List<string> {"repo1", "repo2", "repo3"};
                
                var repositoryObjects = GetDummyRepositoryObjectsForNames(repositoryNames);

                var mockClient = new Mock<IGitHubClient>();
                mockClient.Setup(x => 
                        x.Repository.GetAllForOrg(It.IsAny<string>()))
                    .Returns(Task.FromResult(repositoryObjects));

                var sut = new GitHubService(mockClient.Object);

                var result = await sut.GetRepositoriesForOrganizationAsync();
                var resultNames = result.Select(repoResult => repoResult.Name).ToList();

                resultNames.Should().Contain(repositoryNames);

            }

            private static IReadOnlyList<Repository> GetDummyRepositoryObjectsForNames(List<string> repositoryNames)
            {
                var repositoryObjects = repositoryNames.Select(repoName =>
                {
                    var repo = new RepositoryBuilder().WithName(repoName).Build();
                    return repo;
                }).ToList().As<IReadOnlyList<Repository>>();
                return repositoryObjects;
            }
        }

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
}
