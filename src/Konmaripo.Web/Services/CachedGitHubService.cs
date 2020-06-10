using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Konmaripo.Web.Models;

namespace Konmaripo.Web.Services
{
    public class CachedGitHubService
    {
        private IGitHubService _gitHubService;
        public CachedGitHubService(IGitHubService gitHubService)
        {
            _gitHubService = gitHubService ?? throw new ArgumentNullException(nameof(gitHubService));
        }

        public async Task<List<GitHubRepo>> GetRepositoriesForOrganizationAsync()
        {
            var result = await _gitHubService.GetRepositoriesForOrganizationAsync();
            return result;
        }
    }
}
