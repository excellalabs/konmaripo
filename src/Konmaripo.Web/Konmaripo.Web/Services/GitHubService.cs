using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Konmaripo.Web.Models;
using Octokit;

namespace Konmaripo.Web.Services
{
    public class GitHubService
    {
        private IGitHubClient _githubClient;
        public GitHubService(IGitHubClient githubClient)
        {
            _githubClient = githubClient ?? throw new ArgumentNullException(nameof(githubClient));
        }

        public async Task<List<GitHubRepo>> GetRepositoriesForOrganizationAsync()
        {
            var repos = await _githubClient.Repository.GetAllForOrg("");

            return repos.Select(x => new GitHubRepo(x.Name)).ToList();
        }
    }
}
