using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Konmaripo.Web.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Octokit;

namespace Konmaripo.Web.Services
{
    public class GitHubService : IGitHubService
    {
        private readonly IGitHubClient _githubClient;
        private readonly GitHubSettings _gitHubSettings;
        private readonly ILogger<GitHubService> _logger;
        public GitHubService(IGitHubClient githubClient, IOptions<GitHubSettings> githubSettings, ILogger<GitHubService> logger)
        {
            _githubClient = githubClient ?? throw new ArgumentNullException(nameof(githubClient));
            _gitHubSettings = githubSettings?.Value ?? throw new ArgumentNullException(nameof(githubSettings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
    }
}
