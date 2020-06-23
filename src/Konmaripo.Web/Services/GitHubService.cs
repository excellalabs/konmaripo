using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Konmaripo.Web.Models;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.Extensions.Options;
using Octokit;
using Serilog;

namespace Konmaripo.Web.Services
{
    public interface IMassIssueCreator
    {
        List<Task> CreateIssue(NewIssue issue, List<GitHubRepo> repoList);
    }
    public class MassIssueCreator
    {
        private readonly System.Threading.SemaphoreSlim _batcher = new System.Threading.SemaphoreSlim(10, 10);
        private readonly IGitHubService _gitHubService;
        private readonly ILogger _logger;

        public MassIssueCreator(IGitHubService gitHubService, ILogger logger)
        {
            _gitHubService = gitHubService;
            _logger = logger;
        }

        public List<Task> CreateIssue(NewIssue issue, List<GitHubRepo> repoList)
        {
            _logger.Information("Queuing issues for {RepoCount}", repoList);
            var taskList = new List<Task>();

            repoList.ForEach(x=> taskList.Add(CreateIssue(issue, x.Id)));

            _logger.Information("Added tasks");

            return taskList;
        }
        private async Task CreateIssue(NewIssue issue, long repoId)
        {
            await _batcher.WaitAsync();

            try
            {
                await _gitHubService.CreateIssueInRepo(issue, repoId);
            }
            catch (Exception e)
            {
                _logger.Error(e, "An error occurred while creating an issue in repoId {RepoId}", repoId);
            }
            finally
            {
                _batcher.Release();
            }
        }
    }

    public class GitHubService : IGitHubService
    {
        private readonly IGitHubClient _githubClient;
        private readonly GitHubSettings _gitHubSettings;
        private readonly ILogger _logger;
        public GitHubService(IGitHubClient githubClient, IOptions<GitHubSettings> githubSettings, ILogger logger)
        {
            _githubClient = githubClient ?? throw new ArgumentNullException(nameof(githubClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _gitHubSettings = githubSettings?.Value ?? throw new ArgumentNullException(nameof(githubSettings));
        }

        public async Task<List<GitHubRepo>> GetRepositoriesForOrganizationAsync()
        {
            var orgName = _gitHubSettings.OrganizationName;

            var repos = await _githubClient.Repository.GetAllForOrg(orgName);

            return repos.Select(x => new GitHubRepo(x.Id, x.Name, x.StargazersCount, x.Archived, x.ForksCount, x.OpenIssuesCount, x.CreatedAt, x.UpdatedAt, x.Description, x.Private, x.PushedAt, x.HtmlUrl)).ToList();
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
            var makeArchived = new RepositoryUpdate(repoName)
            {
                Archived = true
            };

            await _githubClient.Repository.Edit(repoId, makeArchived);
        }

        public async Task<RepoQuota> GetRepoQuotaForOrg()
        {
            var org = await _githubClient.Organization.Get(_gitHubSettings.OrganizationName);
            return new RepoQuota(org.Plan.PrivateRepos, org.OwnedPrivateRepos);
        }

        public int RemainingAPIRequests()
        {
            return _githubClient.GetLastApiInfo().RateLimit.Remaining;
        }

        public Task CreateIssueInRepo(NewIssue issue, long repoId)
        {
            return _githubClient.Issue.Create(repoId, issue);
        }
    }
}
