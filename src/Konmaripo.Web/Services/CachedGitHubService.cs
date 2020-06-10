using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Konmaripo.Web.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Konmaripo.Web.Services
{
    public class CachedGitHubService : IGitHubService
    {
        private const string RepoCacheKey = "repoList";

        private readonly IGitHubService _gitHubService;
        private readonly IMemoryCache _memoryCache;

        public CachedGitHubService(IGitHubService gitHubService, IMemoryCache memoryCache)
        {
            _gitHubService = gitHubService ?? throw new ArgumentNullException(nameof(gitHubService));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        public async Task<List<GitHubRepo>> GetRepositoriesForOrganizationAsync()
        {
            var gotFromCache = _memoryCache.TryGetValue(RepoCacheKey, out List<GitHubRepo> cachedRepoList);
            if (gotFromCache) { return cachedRepoList; }

            var repoList = await _gitHubService.GetRepositoriesForOrganizationAsync();
            var cacheEntry = _memoryCache.Set(RepoCacheKey, repoList, TimeSpan.FromDays(1));

            return cacheEntry;
        }

        public Task<ExtendedRepoInformation> GetExtendedRepoInformationFor(long repoId)
        {
            // TODO: Actually add caching
            return _gitHubService.GetExtendedRepoInformationFor(repoId);
        }

        public async Task CreateArchiveIssueInRepo(long repoId, string currentUser)
        {
            await _gitHubService.CreateArchiveIssueInRepo(repoId, currentUser);
        }

        public async Task ArchiveRepository(long repoId, string repoName)
        {
            await _gitHubService.ArchiveRepository(repoId, repoName);
        }
    }
}
