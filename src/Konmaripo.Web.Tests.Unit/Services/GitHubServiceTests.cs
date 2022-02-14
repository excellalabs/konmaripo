using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Konmaripo.Web.Models;
using Konmaripo.Web.Services;
using Konmaripo.Web.Tests.Unit.Helpers;
using Microsoft.Extensions.Options;
using Moq;
using Octokit;
using Serilog;
using Xunit;

namespace Konmaripo.Web.Tests.Unit.Services
{

    public class GitHubServiceTests
    {
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
        public class Ctor
        {
            readonly Mock<IGitHubClient> _dummyClient = new Mock<IGitHubClient>();
            readonly GitHubSettings _dummySettingsObj = new GitHubSettings();
            readonly Mock<IOptions<GitHubSettings>> _dummySettings = new Mock<IOptions<GitHubSettings>>();
            readonly Mock<ILogger> _mockLogger = new Mock<ILogger>();
            readonly Mock<IRepositoryArchiver> _mockArchiver = new Mock<IRepositoryArchiver>();

            public Ctor()
            {
                _dummySettings.Setup(x => x.Value).Returns(_dummySettingsObj);
            }

            [Fact]
            public void HasAllDependencies_DoesntThrow()
            {
                Action sut = () => new GitHubService(githubClient: _dummyClient.Object, _dummySettings.Object, _mockLogger.Object, _mockArchiver.Object);

                sut.Should().NotThrow();
            }

            [Fact]
            public void NullGitHubClient_ThrowsException()
            {
                Action sut = () => new GitHubService(githubClient: null, _dummySettings.Object, _mockLogger.Object, _mockArchiver.Object);

                sut.Should().Throw<ArgumentNullException>()
                    .And.ParamName.Should().Be("githubClient");
            }

            [Fact]
            public void NullOptions_ThrowsException()
            {
                Action sut = () => new GitHubService(githubClient: _dummyClient.Object, null, _mockLogger.Object, _mockArchiver.Object);

                sut.Should().Throw<ArgumentNullException>()
                    .And.ParamName.Should().Be("githubSettings");
            }

            [Fact]
            public void NullLogger_ThrowsException()
            {
                Action sut = () => new GitHubService(githubClient: _dummyClient.Object, _dummySettings.Object, null, _mockArchiver.Object);

                sut.Should().Throw<ArgumentNullException>()
                    .And.ParamName.Should().Be("logger");
            }

        }

        public class GetRepositoriesForOrganization
        {
            private readonly GitHubService _sut;
            private readonly Mock<IGitHubClient> _mockClient;
            private readonly Mock<IRepositoriesClient> _mockRepoClient;
            private readonly GitHubSettings _settingsObject = new GitHubSettings();
            readonly Mock<ILogger> _mockLogger;
            readonly Mock<IRepositoryArchiver> _mockArchiver = new Mock<IRepositoryArchiver>();

            public GetRepositoriesForOrganization()
            {
                _mockClient = new Mock<IGitHubClient>();
                _mockRepoClient = new Mock<IRepositoriesClient>();
                _mockLogger = new Mock<ILogger>();

                _mockClient.Setup(x => x.Repository).Returns(_mockRepoClient.Object);
 
                var mockSettings = new Mock<IOptions<GitHubSettings>>();
                mockSettings.Setup(x => x.Value).Returns(_settingsObject);
                _sut = new GitHubService(_mockClient.Object, mockSettings.Object, _mockLogger.Object, _mockArchiver.Object);
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
            public async Task ReturnsThePushedDateFromTheGithubClient()
            {
                var dateList = new List<DateTimeOffset> { DateTimeOffset.Now.AddDays(-1), DateTimeOffset.Now.AddDays(-12), DateTimeOffset.Now.AddDays(-123) };

                var repositoryObjects = dateList.Select(updatedDate =>
                {
                    var repo = new RepositoryBuilder().WithPushedDate(updatedDate).Build();
                    return repo;
                }).ToList().As<IReadOnlyList<Repository>>();

                _mockRepoClient.Setup(x =>
                        x.GetAllForOrg(It.IsAny<string>()))
                    .Returns(Task.FromResult(repositoryObjects));

                var result = await _sut.GetRepositoriesForOrganizationAsync();
                var resultDates = result.Select(repoResult => repoResult.PushedDate.Value).ToList();

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
                var descriptionList = new List<string> { "blah", "blahblah", "blahblahblah"};

                var repositoryObjects = descriptionList.Select(desc =>
                {
                    var repo = new RepositoryBuilder().WithDescription(desc).Build();
                    return repo;
                }).ToList().As<IReadOnlyList<Repository>>();

                _mockRepoClient.Setup(x =>
                        x.GetAllForOrg(It.IsAny<string>()))
                    .Returns(Task.FromResult(repositoryObjects));

                var result = await _sut.GetRepositoriesForOrganizationAsync();
                var resultDescriptions = result.Select(repoResult => repoResult.Description).ToList();

                resultDescriptions.Should().BeEquivalentTo(descriptionList);
            }

            [Fact]
            public async Task ReturnsWhetherPrivateFromTheGithubClient()
            {
                var isPrivateList = new List<bool> { false, true, true };

                var repositoryObjects = isPrivateList.Select(isPrivate =>
                {
                    var repo = new RepositoryBuilder().WithIsPrivateOf(isPrivate).Build();
                    return repo;
                }).ToList().As<IReadOnlyList<Repository>>();

                _mockRepoClient.Setup(x =>
                        x.GetAllForOrg(It.IsAny<string>()))
                    .Returns(Task.FromResult(repositoryObjects));

                var result = await _sut.GetRepositoriesForOrganizationAsync();
                var resultPrivateFlags = result.Select(repoResult => repoResult.IsPrivate).ToList();

                resultPrivateFlags.Should().BeEquivalentTo(isPrivateList);
            }

            [Fact]
            public async Task ReturnsRepoUrlFromTheGithubClient()
            {
                var urlList = new List<string> { "url1", "url2", "url3" };

                var repositoryObjects = urlList.Select(url =>
                {
                    var repo = new RepositoryBuilder().WithUrlOf(url).Build();
                    return repo;
                }).ToList().As<IReadOnlyList<Repository>>();

                _mockRepoClient.Setup(x =>
                        x.GetAllForOrg(It.IsAny<string>()))
                    .Returns(Task.FromResult(repositoryObjects));

                var result = await _sut.GetRepositoriesForOrganizationAsync();
                var resultUrls = result.Select(repoResult => repoResult.RepoUrl).ToList();

                resultUrls.Should().BeEquivalentTo(urlList);
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

        public class CreateArchiveIssueInRepo
        {
            private readonly GitHubService _sut;
            private readonly Mock<IGitHubClient> _mockClient;
            private readonly Mock<IRepositoriesClient> _mockRepoClient;
            private readonly GitHubSettings _settingsObject = new GitHubSettings();
            readonly Mock<ILogger> _mockLogger;
            readonly Mock<IRepositoryArchiver> _mockArchiver = new Mock<IRepositoryArchiver>();

            public CreateArchiveIssueInRepo()
            {
                _mockClient = new Mock<IGitHubClient>();
                _mockRepoClient = new Mock<IRepositoriesClient>();
                _mockLogger = new Mock<ILogger>();

                _mockClient.Setup(x => x.Repository).Returns(_mockRepoClient.Object);

                var mockSettings = new Mock<IOptions<GitHubSettings>>();
                mockSettings.Setup(x => x.Value).Returns(_settingsObject);
                _sut = new GitHubService(_mockClient.Object, mockSettings.Object, _mockLogger.Object, _mockArchiver.Object);
            }

            [Fact]
            public void WhenIssuesAreDisabled_DoesntThrowException()
            {
                const int idThatDoesntMatter = 0;
                const string nameThatDoesntMatter = "name";

                var issuesDisabledException = new ApiException("Issues are disabled for this repo", HttpStatusCode.BadRequest);

                var mockIssuesClient = new Mock<IIssuesClient>();
                mockIssuesClient.Setup(x => x.Create(It.IsAny<long>(), It.IsAny<NewIssue>()))
                    .Throws(issuesDisabledException);

                _mockClient.Setup(x => x.Issue).Returns(mockIssuesClient.Object);

                Func<Task> act = async () => await _sut.CreateArchiveIssueInRepo(idThatDoesntMatter, nameThatDoesntMatter);

                act.Should().NotThrowAsync();
            }

            [Fact]
            public void WhenClientThrowsErrorUnrelatedToDisabledIssues_ThrowsException()
            {
                const int idThatDoesntMatter = 0;
                const string nameThatDoesntMatter = "name";

                // ReSharper disable once StringLiteralTypo
                var messageUnrelatedToDisabledIssues = "Blahblahblahblah";
                var issuesDisabledException = new ApiException(messageUnrelatedToDisabledIssues, HttpStatusCode.BadRequest);

                var mockIssuesClient = new Mock<IIssuesClient>();
                mockIssuesClient.Setup(x => x.Create(It.IsAny<long>(), It.IsAny<NewIssue>()))
                    .Throws(issuesDisabledException);

                _mockClient.Setup(x => x.Issue).Returns(mockIssuesClient.Object);

                Func<Task> act = async () => await _sut.CreateArchiveIssueInRepo(idThatDoesntMatter, nameThatDoesntMatter);

                act.Should().ThrowAsync<ApiException>();
            }

            [Fact]
            public async Task WhenIssuesAreDisabled_LogsWarning()
            {
                const long repositoryId = 1234;

                const string nameThatDoesntMatter = "name";

                var issuesDisabledException = new ApiException("Issues are disabled for this repo", HttpStatusCode.BadRequest);

                var mockIssuesClient = new Mock<IIssuesClient>();
                mockIssuesClient.Setup(x => x.Create(It.IsAny<long>(), It.IsAny<NewIssue>()))
                    .Throws(issuesDisabledException);

                _mockClient.Setup(x => x.Issue).Returns(mockIssuesClient.Object);

                await _sut.CreateArchiveIssueInRepo(repositoryId, nameThatDoesntMatter);

                _mockLogger.Verify(x=>x.Warning("Issues are disabled for repository ID '{RepositoryID}'; could not create archive issue.", repositoryId), Times.Once);
            }
        }

        public class GetRepoQuotaForOrg
        {
            private readonly GitHubService _sut;
            private readonly Mock<IGitHubClient> _mockClient;
            private readonly Mock<IOrganizationsClient> _mockOrgClient;
            private readonly GitHubSettings _settingsObject = new GitHubSettings();
            readonly Mock<IRepositoryArchiver> _mockArchiver = new Mock<IRepositoryArchiver>();

            readonly Mock<ILogger> _mockLogger;

            public GetRepoQuotaForOrg()
            {
                _mockClient = new Mock<IGitHubClient>();
                _mockOrgClient = new Mock<IOrganizationsClient>();
                _mockLogger = new Mock<ILogger>();

                _mockClient.Setup(x => x.Organization).Returns(_mockOrgClient.Object);

                var mockSettings = new Mock<IOptions<GitHubSettings>>();
                mockSettings.Setup(x => x.Value).Returns(_settingsObject);
                _sut = new GitHubService(_mockClient.Object, mockSettings.Object, _mockLogger.Object,
                    _mockArchiver.Object);
            }

            [Fact]
            public async Task UsesOrgFromSettings()
            {
                var testOrgName = "MyTestOrg";

                _settingsObject.OrganizationName = testOrgName;

                _mockOrgClient.Setup(x =>
                        x.Get(It.IsAny<string>()))
                    .Returns(Task.FromResult(new OrganizationBuilder().Build()));

                await _sut.GetRepoQuotaForOrg();

                _mockClient.Verify(x => x.Organization.Get(testOrgName), Times.Once);
            }

            [Fact]
            public async Task GetsRepoLimitFromOrganizationPlan()
            {
                var privateRepoLimit = 123;

                var orgResult = new OrganizationBuilder()
                    .WithPrivateRepoLimit(privateRepoLimit)
                    .Build();

                _mockOrgClient.Setup(x =>
                        x.Get(It.IsAny<string>()))
                    .Returns(Task.FromResult(orgResult));

                var result = await _sut.GetRepoQuotaForOrg();

                result.PrivateRepoLimit.Should().Be(privateRepoLimit);
            }

            [Fact]
            public async Task GetsPrivateRepositoryCountFromOrganizationInfo()
            {
                var privateRepoCount = 123;

                var orgResult = new OrganizationBuilder()
                    .WithPrivateRepoCount(privateRepoCount)
                    .Build();

                _mockOrgClient.Setup(x =>
                        x.Get(It.IsAny<string>()))
                    .Returns(Task.FromResult(orgResult));

                var result = await _sut.GetRepoQuotaForOrg();

                result.PrivateRepoCount.Should().Be(privateRepoCount);
            }
        }

    }
}
