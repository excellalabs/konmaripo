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
                
                var repositoryObjects = repositoryNames.Select(repoName =>
                {
                    var repo = new RepositoryBuilder().WithName(repoName).Build();
                    return repo;
                }).ToList().As<IReadOnlyList<Repository>>();

                _mockRepoClient.Setup(x => 
                        x.GetAllForOrg(It.IsAny<string>()))
                    .Returns(Task.FromResult(repositoryObjects));

                var result = await _sut.GetRepositoriesForOrganizationAsync();
                var resultNames = result.Select(repoResult => repoResult.Name).ToList();

                resultNames.Should().BeEquivalentTo(repositoryNames);
            }

            [Fact]
            public async Task ReturnsTheNumberOfStarsFromTheGithubClient()
            {
                var starCountList = new List<int> { 1, 123, 123_456 };

                var repositoryObjects = starCountList.Select(starCount =>
                {
                    var repo = new RepositoryBuilder().WithStarCount(starCount).Build();
                    return repo;
                }).ToList().As<IReadOnlyList<Repository>>();

                _mockRepoClient.Setup(x =>
                        x.GetAllForOrg(It.IsAny<string>()))
                    .Returns(Task.FromResult(repositoryObjects));

                var result = await _sut.GetRepositoriesForOrganizationAsync();
                var resultStars = result.Select(repoResult => repoResult.StarCount).ToList();

                resultStars.Should().BeEquivalentTo(starCountList);
            }

            [Fact]
            public async Task ReturnsIsArchivedFromTheGithubClient()
            {
                var archivedList = new List<bool> { false, true, true };

                var repositoryObjects = archivedList.Select(archived =>
                {
                    var repo = new RepositoryBuilder().WithArchivedOf(archived).Build();
                    return repo;
                }).ToList().As<IReadOnlyList<Repository>>();

                _mockRepoClient.Setup(x =>
                        x.GetAllForOrg(It.IsAny<string>()))
                    .Returns(Task.FromResult(repositoryObjects));

                var result = await _sut.GetRepositoriesForOrganizationAsync();
                var resultArchivedFlags = result.Select(repoResult => repoResult.IsArchived).ToList();

                resultArchivedFlags.Should().BeEquivalentTo(archivedList);
            }

            [Fact]
            public async Task ReturnsTheNumberOfForksFromTheGithubClient()
            {
                var forkCountList = new List<int> { 1, 123, 123_456 };

                var repositoryObjects = forkCountList.Select(forkCount =>
                {
                    var repo = new RepositoryBuilder().WithForkCount(forkCount).Build();
                    return repo;
                }).ToList().As<IReadOnlyList<Repository>>();

                _mockRepoClient.Setup(x =>
                        x.GetAllForOrg(It.IsAny<string>()))
                    .Returns(Task.FromResult(repositoryObjects));

                var result = await _sut.GetRepositoriesForOrganizationAsync();
                var resultForkCounts = result.Select(repoResult => repoResult.ForkCount).ToList();

                resultForkCounts.Should().BeEquivalentTo(forkCountList);
            }

            [Fact]
            public async Task ReturnsTheNumberOfOpenIssuesFromTheGithubClient()
            {
                var openIssueCountList = new List<int> { 1, 123, 123_456 };

                var repositoryObjects = openIssueCountList.Select(openIssueCount =>
                {
                    var repo = new RepositoryBuilder().WithOpenIssues(openIssueCount).Build();
                    return repo;
                }).ToList().As<IReadOnlyList<Repository>>();

                _mockRepoClient.Setup(x =>
                        x.GetAllForOrg(It.IsAny<string>()))
                    .Returns(Task.FromResult(repositoryObjects));

                var result = await _sut.GetRepositoriesForOrganizationAsync();
                var resultIssueCounts = result.Select(repoResult => repoResult.OpenIssueCount).ToList();

                resultIssueCounts.Should().BeEquivalentTo(openIssueCountList);
            }

            [Fact]
            public async Task ReturnsTheCreatedDateFromTheGithubClient()
            {
                var dateList = new List<DateTimeOffset> { DateTimeOffset.Now.AddDays(-1), DateTimeOffset.Now.AddDays(-12), DateTimeOffset.Now.AddDays(-123) };

                var repositoryObjects = dateList.Select(createdDate =>
                {
                    var repo = new RepositoryBuilder().WithCreatedDate(createdDate).Build();
                    return repo;
                }).ToList().As<IReadOnlyList<Repository>>();

                _mockRepoClient.Setup(x =>
                        x.GetAllForOrg(It.IsAny<string>()))
                    .Returns(Task.FromResult(repositoryObjects));

                var result = await _sut.GetRepositoriesForOrganizationAsync();
                var resultDates = result.Select(repoResult => repoResult.CreatedDate).ToList();

                resultDates.Should().BeEquivalentTo(dateList);
            }

            [Fact]
            public async Task ReturnsTheUpdatedDateFromTheGithubClient()
            {
                var dateList = new List<DateTimeOffset> { DateTimeOffset.Now.AddDays(-1), DateTimeOffset.Now.AddDays(-12), DateTimeOffset.Now.AddDays(-123) };

                var repositoryObjects = dateList.Select(updatedDate =>
                {
                    var repo = new RepositoryBuilder().WithUpdatedDate(updatedDate).Build();
                    return repo;
                }).ToList().As<IReadOnlyList<Repository>>();

                _mockRepoClient.Setup(x =>
                        x.GetAllForOrg(It.IsAny<string>()))
                    .Returns(Task.FromResult(repositoryObjects));

                var result = await _sut.GetRepositoriesForOrganizationAsync();
                var resultDates = result.Select(repoResult => repoResult.UpdatedDate).ToList();

                resultDates.Should().BeEquivalentTo(dateList);
            }

            [Fact]
            public async Task ReturnsTheRepositoryIdFromTheGithubClient()
            {
                var idList = new List<long> { 1, 123, 12345 };

                var repositoryObjects = idList.Select(repoId =>
                {
                    var repo = new RepositoryBuilder().WithId(repoId).Build();
                    return repo;
                }).ToList().As<IReadOnlyList<Repository>>();

                _mockRepoClient.Setup(x =>
                        x.GetAllForOrg(It.IsAny<string>()))
                    .Returns(Task.FromResult(repositoryObjects));

                var result = await _sut.GetRepositoriesForOrganizationAsync();
                var resultIds = result.Select(repoResult => repoResult.Id).ToList();

                resultIds.Should().BeEquivalentTo(idList);
            }

            [Fact]
            public async Task ReturnsTheDescriptionFromTheGithubClient()
            {
                throw new NotImplementedException();
            }

            [Fact]
            public async Task ReturnsWhetherPrivateFromTheGithubClient()
            {
                throw new NotImplementedException();
            }

            [Fact]
            public async Task UsesTheOrganizationNameFromSettings()
            {
                var testOrgName = "MyTestOrg";

                _settingsObject.OrganizationName = testOrgName;

                _mockRepoClient.Setup(x =>
                        x.GetAllForOrg(It.IsAny<string>()))
                    .Returns(Task.FromResult(new List<Repository>().As<IReadOnlyList<Repository>>()));

                await _sut.GetRepositoriesForOrganizationAsync();

                _mockClient.Verify(x=>x.Repository.GetAllForOrg(testOrgName), Times.Once);
            }

            [Fact]
            public async Task ReturnsEmptyListWhenGitHubClientReturnsEmptyList()
            {
                _mockRepoClient.Setup(x =>
                        x.GetAllForOrg(It.IsAny<string>()))
                    .Returns(Task.FromResult(new List<Repository>().ToList().As<IReadOnlyList<Repository>>()));

                var result = await _sut.GetRepositoriesForOrganizationAsync();
                var resultNames = result.Select(repoResult => repoResult.Name).ToList();

                resultNames.Should().BeEmpty();
            }
        }

    }
}
