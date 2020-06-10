using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Konmaripo.Web.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Konmaripo.Web.Services
{
    public class CachedGitHubService
    {
        private IGitHubService _gitHubService;
        private IMemoryCache _memoryCache;
        public CachedGitHubService(IGitHubService gitHubService, IMemoryCache memoryCache)
        {
            _gitHubService = gitHubService ?? throw new ArgumentNullException(nameof(gitHubService));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        public async Task<List<GitHubRepo>> GetRepositoriesForOrganizationAsync()
        {
            var result = await _gitHubService.GetRepositoriesForOrganizationAsync();
            return result;
        }
    }
}
