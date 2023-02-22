using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Konmaripo.Web.Models;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.Extensions.Options;
using Octokit;
using Serilog;
using TimeZoneConverter;
using FileMode = System.IO.FileMode;

namespace Konmaripo.Web.Services
{
    public static class ExtensionMethods
    {
        public static GitHubRepo ToKonmaripoRepo(this Repository x)
        {
            return new GitHubRepo(x.Id, x.Name, x.StargazersCount, x.Archived, x.ForksCount, x.OpenIssuesCount, x.CreatedAt, x.UpdatedAt, x.Description, x.Private, x.PushedAt, x.HtmlUrl, x.SubscribersCount, x.Topics);
        }
    }

    public class GitHubService : IGitHubService
    {
        private readonly IGitHubClient _githubClient;
        private readonly GitHubSettings _gitHubSettings;
        private readonly ILogger _logger;
        private readonly IRepositoryArchiver _archiver;
        private const string START_PATH = "./Data"; // TODO: Extract to config

        public GitHubService(IGitHubClient githubClient, IOptions<GitHubSettings> githubSettings, ILogger logger, IRepositoryArchiver archiver)
        {
            _githubClient = githubClient ?? throw new ArgumentNullException(nameof(githubClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _gitHubSettings = githubSettings?.Value ?? throw new ArgumentNullException(nameof(githubSettings));
            _archiver = archiver ?? throw new ArgumentNullException(nameof(archiver));
        }

        public async Task<List<GitHubRepo>> GetRepositoriesForTeam(string teamName)
        {
            var allTeams = await GetAllTeams();
            var teamId = allTeams.Single(x => x.Name.Equals(teamName, StringComparison.InvariantCultureIgnoreCase)).Id;

            var repos = await _githubClient.Organization.Team.GetAllRepositories(teamId);
            return repos.Select(x => x.ToKonmaripoRepo()).ToList();
        }

        public async Task AddAllOrgTeamToRepos(List<GitHubRepo> vmRepositoriesToAddAccessTo, string teamName)
        {
            var allTeams = await GetAllTeams();
            var teamId = allTeams.Single(x => x.Name.Equals(teamName, StringComparison.InvariantCultureIgnoreCase)).Id;

            foreach (var repo in vmRepositoriesToAddAccessTo)
            {
                await _githubClient.Organization.Team.AddRepository(teamId, _gitHubSettings.OrganizationName, repo.Name);
            }
        }

        public async Task RemoveAllOrgTeamFromRepos(List<GitHubRepo> vmRepositoriesToRemoveAccessFrom, string teamName)
        {
            var allTeams = await GetAllTeams();
            var teamId = allTeams.Single(x => x.Name.Equals(teamName, StringComparison.InvariantCultureIgnoreCase)).Id;

            foreach (var repo in vmRepositoriesToRemoveAccessFrom)
            {
                await _githubClient.Organization.Team.RemoveRepository(teamId, _gitHubSettings.OrganizationName, repo.Name);
            }
        }

        public async Task<List<GitHubRepo>> GetRepositoriesForOrganizationAsync()
        {
            var orgName = _gitHubSettings.OrganizationName;

            var repos = await _githubClient.Repository.GetAllForOrg(orgName);

            return repos.Select(x => x.ToKonmaripoRepo()).ToList();
        }

        public async Task<ExtendedRepoInformation> GetExtendedRepoInformationFor(long repoId)
        {
            var watchers = await _githubClient.Activity.Watching.GetAllWatchers(repoId);
            var views = await _githubClient.Repository.Traffic.GetViews(repoId, new RepositoryTrafficRequest(TrafficDayOrWeek.Week));
            var commitActivity = await _githubClient.Repository.Statistics.GetCommitActivity(repoId);

            var commitActivityInLast4Weeks = commitActivity.Activity.OrderByDescending(x => x.WeekTimestamp).Take(4).Sum(x => x.Total);
            var extendedRepoInfo = new ExtendedRepoInformation(repoId, watchers.Count, views.Count, commitActivityInLast4Weeks);

            return extendedRepoInfo;
        }

        public async Task CreateArchiveIssueInRepo(long repoId, string currentUser)
        {
            var newIssue = new NewIssue("This repository is being archived")
            {
                Body = @$"Archive process initiated by {currentUser} via the Konmaripo tool."
            };

            try
            {
                await _githubClient.Issue.Create(repoId, newIssue);
            }
            catch(ApiException ex)
            {
                _logger.Warning("Issues are disabled for repository ID '{RepositoryID}'; could not create archive issue.", repoId);
                if (ex.Message != "Issues are disabled for this repo")
                {
                    throw;
                }
            }
        }

        public async Task ArchiveRepository(long repoId, string repoName)
        {
            var makeArchived = new RepositoryUpdate();
            makeArchived.Archived = true;
            makeArchived.Name = repoName;

            await _githubClient.Repository.Edit(repoId, makeArchived);
        }

        public async Task<RepoQuota> GetRepoQuotaForOrg()
        {
            var org = await _githubClient.Organization.Get(_gitHubSettings.OrganizationName);
            return new RepoQuota(org.Plan.PrivateRepos, org.OwnedPrivateRepos);
        }

        public async Task<ZippedRepositoryStreamResult> ZippedRepositoryStreamAsync(string repoName)
        {
            var pathToFullRepo = _archiver.CloneRepositoryWithTagsAndBranches(repoName);

            var destinationArchiveFilePath = await GenerateDestinationArchiveFilePath(START_PATH, repoName);
            var destinationArchiveFileName = Path.GetFileName(destinationArchiveFilePath);
            // TODO: Make async
            ZipFile.CreateFromDirectory(pathToFullRepo.Value, destinationArchiveFilePath, CompressionLevel.Fastest, false);

            Directory.Delete(pathToFullRepo.Value, true);

            var stream = new FileStream(destinationArchiveFilePath, FileMode.Open, FileAccess.Read, FileShare.None, 4096, FileOptions.DeleteOnClose);

            return new ZippedRepositoryStreamResult(stream, destinationArchiveFileName);
        }

        private async Task<string> GenerateDestinationArchiveFilePath(string startPath, string repoName)
        {
            var allInfo = await GetRepositoriesForOrganizationAsync();
            var repoInfo = allInfo.Single(x => x.Name.Equals(repoName, StringComparison.InvariantCultureIgnoreCase));

            var dateString = repoInfo.PushedDate.HasValue ? $"_LastPushed{repoInfo.PushedDate.Value:yyyy-MM-dd}" : "";
            return Path.Combine(startPath, $"{repoName}{dateString}.zip");
        }

        public int RemainingAPIRequests()
        {
            return _githubClient.GetLastApiInfo().RateLimit.Remaining;
        }

        public Task CreateIssueInRepo(NewIssue issue, long repoId)
        {
            return _githubClient.Issue.Create(repoId, issue);
        }

        public DateTimeOffset APITokenResetTime()
        {
            var reset = _githubClient.GetLastApiInfo().RateLimit.Reset;
            var timeZoneToConvertTo = TZConvert.GetTimeZoneInfo(_gitHubSettings.TimeZoneDisplayId);

            var resultingTime = TimeZoneInfo.ConvertTime(reset, timeZoneToConvertTo);

            return resultingTime;
        }

        public Task DeleteRepository(long repoId)
        {
            _logger.Warning("Deleting Repository {RepoId}", repoId);
            return _githubClient.Repository.Delete(repoId);
        }

        public async Task<bool> TeamExists(string teamName)
        {
            var allTeams = await _githubClient.Organization.Team.GetAll(_gitHubSettings.OrganizationName);

            return allTeams.Any(x => x.Name.Equals(teamName, StringComparison.InvariantCultureIgnoreCase));
        }

        public Task CreateTeam(string teamName, string teamDescription)
        {
            var newTeam = new NewTeam(teamName) {Description = teamDescription};

            return _githubClient.Organization.Team.Create(_gitHubSettings.OrganizationName, newTeam);
        }

        public async Task<List<User>> GetUsersNotInTeam(string teamName)
        {
            var allTeams = await GetAllTeams();
            var allOrgMembers = await GetAllUsers();
            var teamId = allTeams.Single(x => x.Name.Equals(teamName, StringComparison.InvariantCultureIgnoreCase)).Id;

            var teamMembers = await GetTeamMembers(teamId);

            return allOrgMembers.Except(teamMembers, new OctokitUserEqualityComparer()).ToList();
        }

        public Task<IReadOnlyList<User>> GetAllUsers()
        {
            return _githubClient.Organization.Member.GetAll(_gitHubSettings.OrganizationName);
        }

        public Task<IReadOnlyList<Team>> GetAllTeams()
        {
            return _githubClient.Organization.Team.GetAll(_gitHubSettings.OrganizationName);
        }

        public Task<IReadOnlyList<User>> GetTeamMembers(int teamId)
        {
            return _githubClient.Organization.Team.GetAllMembers(teamId);
        }

        public async Task AddMembersToTeam(string teamName, List<string> loginsToAdd)
        {
            var allTeams = await GetAllTeams();
            var theTeam = allTeams.Single(x => x.Name.Equals(teamName, StringComparison.InvariantCultureIgnoreCase));

            await AddMembersToTeam(theTeam.Id, loginsToAdd);
        }

        public async Task AddMembersToTeam(int teamId, List<string> loginsToAdd)
        {
            var request = new UpdateTeamMembership(TeamRole.Member);
            foreach (var login in loginsToAdd)
            {
                await _githubClient.Organization.Team.AddOrEditMembership(teamId, login, request);
            }
        }

        public async Task<List<GitHubRepo>> GetRepositoriesWithTopic(string topicName)
        {
            var allRepos = await GetRepositoriesForOrganizationAsync();

            var reposWithTopic = allRepos.Where(x=>x.Topics.Any(x=>x.Equals(topicName, StringComparison.InvariantCultureIgnoreCase))).ToList();

            return reposWithTopic;
            // TODO Filter repos by topic.
        }
    }
}
