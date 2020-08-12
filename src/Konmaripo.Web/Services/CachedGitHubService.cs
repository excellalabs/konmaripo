using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Functional.Maybe;
using Konmaripo.Web.Models;
using Microsoft.Extensions.Caching.Memory;
using Octokit;

namespace Konmaripo.Web.Services
{
    public class CachedGitHubService : IGitHubService
    {
        private const string RepoCacheKey = "repoList";

        private readonly IGitHubService _gitHubService;
        private readonly IMemoryCache _memoryCache;
        private readonly TimeSpan _cacheTimeout = TimeSpan.FromDays(1);

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
            var cacheEntry = _memoryCache.Set(RepoCacheKey, repoList, _cacheTimeout);

            return cacheEntry;
        }

        public async Task<ExtendedRepoInformation> GetExtendedRepoInformationFor(long repoId)
        {
            var result = await _memoryCache.GetOrCreateAsync($"extendedInfo-{repoId}",
                async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = _cacheTimeout;
                    return await _gitHubService.GetExtendedRepoInformationFor(repoId);
                });

            return result;
        }

        public async Task CreateArchiveIssueInRepo(long repoId, string currentUser)
        {
            await _gitHubService.CreateArchiveIssueInRepo(repoId, currentUser);
        }

        public async Task ArchiveRepository(long repoId, string repoName)
        {
            await _gitHubService.ArchiveRepository(repoId, repoName);

            var repos = _memoryCache.Get<List<GitHubRepo>>(RepoCacheKey);
            var item = repos.First(x => x.Id == repoId);

            var archivedItem = new GitHubRepo(item.Id, item.Name, item.StarCount, true, item.ForkCount, item.OpenIssueCount, item.CreatedDate, item.UpdatedDate, item.Description, item.IsPrivate, item.PushedDate.ToNullable(), item.RepoUrl);
            repos.Remove(item);
            repos.Add(archivedItem);

            _memoryCache.Set(RepoCacheKey, repos, _cacheTimeout);
        }

        public Task<RepoQuota> GetRepoQuotaForOrg()
        {
            return _gitHubService.GetRepoQuotaForOrg();
        }

        public Task<FileStream> ZippedRepositoryStreamAsync(string repoName)
        {
            return _gitHubService.ZippedRepositoryStreamAsync(repoName);
        }
        public int RemainingAPIRequests()
        {
            return _gitHubService.RemainingAPIRequests();
        }

        public Task CreateIssueInRepo(NewIssue issue, long repoId)
        {
            return _gitHubService.CreateIssueInRepo(issue, repoId);
        }

        public DateTimeOffset APITokenResetTime()
        {
            return _gitHubService.APITokenResetTime();
        }
    }
}
