using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
        public class Ctor
        {
            readonly Mock<IGitHubClient> _dummyClient = new Mock<IGitHubClient>();
            readonly Mock<IOptions<GitHubSettings>> _dummySettings = new Mock<IOptions<GitHubSettings>>();

            [Fact]
            public void NullGitHubClient_ThrowsException()
            {
                Action sut = () => new GitHubService(githubClient: null, _dummySettings.Object);

                sut.Should().Throw<ArgumentNullException>()
                    .And.ParamName.Should().Be("githubClient");
            }

            [Fact]
            public void NullOptions_ThrowsException()
            {
                Action sut = () => new GitHubService(githubClient: _dummyClient.Object, null);

                sut.Should().Throw<ArgumentNullException>()
                    .And.ParamName.Should().Be("githubSettings");
            }
        }

        public class GetRepositoriesForOrganization
        {
            private GitHubService _sut;
            private readonly Mock<IGitHubClient> _mockClient;
            private readonly Mock<IRepositoriesClient> _mockRepoClient;
            private readonly Mock<IOptions<GitHubSettings>> _mockSettings;
            private readonly GitHubSettings _settingsObject = new GitHubSettings();

            public GetRepositoriesForOrganization()
            {
                _mockClient = new Mock<IGitHubClient>();
                _mockRepoClient = new Mock<IRepositoriesClient>();

                _mockClient.Setup(x => x.Repository).Returns(_mockRepoClient.Object);
 
                _mockSettings = new Mock<IOptions<GitHubSettings>>();
                _mockSettings.Setup(x => x.Value).Returns(_settingsObject);
                _sut = new GitHubService(_mockClient.Object, _mockSettings.Object);
            }

            [Fact]
            public async Task ReturnsTheRepositoryNamesFromTheGithubClient()
            {
                var repositoryNames = new List<string> {"repo1", "repo2", "repo3"};
                
                var repositoryObjects = GetDummyRepositoryObjectsForNames(repositoryNames);

                _mockRepoClient.Setup(x => 
                        x.GetAllForOrg(It.IsAny<string>()))
                    .Returns(Task.FromResult(repositoryObjects));

                var result = await _sut.GetRepositoriesForOrganizationAsync();
                var resultNames = result.Select(repoResult => repoResult.Name).ToList();

                resultNames.Should().Contain(repositoryNames);
            }

            [Fact]
            public void UsesTheOrganizationNameFromSettings()
            {
                var testOrgName = "MyTestOrg";

                _settingsObject.OrganizationName = testOrgName;

                _sut.GetRepositoriesForOrganizationAsync();

                _mockClient.Verify(x=>x.Repository.GetAllForOrg(testOrgName), Times.Once);
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
