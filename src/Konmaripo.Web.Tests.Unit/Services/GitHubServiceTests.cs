using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Konmaripo.Web.Services;
using Konmaripo.Web.Tests.Unit.Helpers;
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
            private GitHubService _sut;
            private readonly Mock<IGitHubClient> _mockClient;

            public GetRepositoriesForOrganization()
            {
                _mockClient = new Mock<IGitHubClient>();
                _sut = new GitHubService(_mockClient.Object);
            }

            [Fact]
            public async Task ReturnsTheRepositoryNamesFromTheGithubClient()
            {
                var repositoryNames = new List<string> {"repo1", "repo2", "repo3"};
                
                var repositoryObjects = GetDummyRepositoryObjectsForNames(repositoryNames);

                _mockClient.Setup(x => 
                        x.Repository.GetAllForOrg(It.IsAny<string>()))
                    .Returns(Task.FromResult(repositoryObjects));

                var result = await _sut.GetRepositoriesForOrganizationAsync();
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

    }
}
