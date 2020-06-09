using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Konmaripo.Web.Models;
using Microsoft.Extensions.Options;
using Octokit;

namespace Konmaripo.Web.Services
{
    public interface IGitHubService
    {
        Task<List<GitHubRepo>> GetRepositoriesForOrganizationAsync();
    }

    public class GitHubService : IGitHubService
    {
        private IGitHubClient _githubClient;
        private GitHubSettings _gitHubSettings;
        public GitHubService(IGitHubClient githubClient, IOptions<GitHubSettings> githubSettings)
        {
            _githubClient = githubClient ?? throw new ArgumentNullException(nameof(githubClient));
            _gitHubSettings = githubSettings?.Value ?? throw new ArgumentNullException(nameof(githubSettings));
        }

        public async Task<List<GitHubRepo>> GetRepositoriesForOrganizationAsync()
        {
            var orgName = _gitHubSettings.OrganizationName;

            var repos = await _githubClient.Repository.GetAllForOrg(orgName);

            return repos.Select(x => new GitHubRepo(x.Name)).ToList();
        }
    }
}
