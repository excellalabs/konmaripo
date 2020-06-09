using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Konmaripo.Web.Models;
using Konmaripo.Web.Services;
using Konmaripo.Web.Tests.Unit.Helpers;
using Microsoft.Extensions.Options;
using Moq;
using Octokit;
using Xunit;

namespace Konmaripo.Web.Tests.Unit.Services
{

    public class GitHubServiceTests
    {
        public class Ctor
        {
            Mock<IGitHubClient> dummyClient = new Mock<IGitHubClient>();
            Mock<IOptions<GitHubSettings>> dummySettings = new Mock<IOptions<GitHubSettings>>();

            [Fact]
            public void NullGitHubClient_ThrowsException()
            {
                Action sut = () => new GitHubService(githubClient: null, dummySettings.Object);

                sut.Should().Throw<ArgumentNullException>()
                    .And.ParamName.Should().Be("githubClient");
            }

            [Fact]
            public void NullOptions_ThrowsException()
            {
                Action sut = () => new GitHubService(githubClient: dummyClient.Object, null);

                sut.Should().Throw<ArgumentNullException>()
                    .And.ParamName.Should().Be("githubSettings");
            }
        }

        public class GetRepositoriesForOrganization
        {
            private GitHubService _sut;
            private readonly Mock<IGitHubClient> _mockClient;
            private readonly Mock<IOptions<GitHubSettings>> _mockSettings;
            private readonly GitHubSettings _settingsObject = new GitHubSettings();

            public GetRepositoriesForOrganization()
            {
                _mockClient = new Mock<IGitHubClient>();
                _mockSettings = new Mock<IOptions<GitHubSettings>>();
                _mockSettings.Setup(x => x.Value).Returns(_settingsObject);
                _sut = new GitHubService(_mockClient.Object, _mockSettings.Object);
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
