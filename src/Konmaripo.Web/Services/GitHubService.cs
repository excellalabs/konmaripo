using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Konmaripo.Web.Models;
using Microsoft.Extensions.Options;
using Octokit;

namespace Konmaripo.Web.Services
{
    public class GitHubService : IGitHubService
    {
        private readonly IGitHubClient _githubClient;
        private readonly GitHubSettings _gitHubSettings;
        public GitHubService(IGitHubClient githubClient, IOptions<GitHubSettings> githubSettings)
        {
            _githubClient = githubClient ?? throw new ArgumentNullException(nameof(githubClient));
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

            var extendedRepoInfo = new ExtendedRepoInformation(repoId, watchers.Count, views.Count);

            return extendedRepoInfo;
        }
    }
}
