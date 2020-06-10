using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Konmaripo.Web.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Konmaripo.Web.Services
{
    public class CachedGitHubService
    {
        private const string REPO_CACHE_KEY = "repoList";

        private readonly IGitHubService _gitHubService;
        private readonly IMemoryCache _memoryCache;

        public CachedGitHubService(IGitHubService gitHubService, IMemoryCache memoryCache)
        {
            _gitHubService = gitHubService ?? throw new ArgumentNullException(nameof(gitHubService));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        public async Task<List<GitHubRepo>> GetRepositoriesForOrganizationAsync()
        {
            var gotFromCache = _memoryCache.TryGetValue(REPO_CACHE_KEY, out List<GitHubRepo> cachedRepoList);
            if (gotFromCache) { return cachedRepoList; }

            var repoList = await _gitHubService.GetRepositoriesForOrganizationAsync();
            var cacheEntry = _memoryCache.Set(REPO_CACHE_KEY, repoList, TimeSpan.FromDays(1));

            return cacheEntry;
        }
    }
}
