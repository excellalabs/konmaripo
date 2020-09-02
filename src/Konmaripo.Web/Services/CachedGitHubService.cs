using System;
using System.Collections.Generic;
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

        public Task<ZippedRepositoryStreamResult> ZippedRepositoryStreamAsync(string repoName)
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

        public async Task DeleteRepository(long repoId)
        {
            await _gitHubService.DeleteRepository(repoId);

            var repos = _memoryCache.Get<List<GitHubRepo>>(RepoCacheKey);
            var item = repos.First(x => x.Id == repoId);

            repos.Remove(item);

            _memoryCache.Set(RepoCacheKey, repos, _cacheTimeout);
        }

        public Task<bool> TeamExists(string teamName)
        {
            return _gitHubService.TeamExists(teamName);
        }

        public Task CreateTeam(string teamName, string teamDescription)
        {
            return _gitHubService.CreateTeam(teamName, teamDescription);
        }

        public Task<IReadOnlyList<User>> GetAllUsers()
        {
            // TODO: Cache
            return _gitHubService.GetAllUsers();
        }

        public Task<IReadOnlyList<Team>> GetAllTeams()
        {
            // TODO: Cache
            return _gitHubService.GetAllTeams();
        }
        public Task<IReadOnlyList<User>> GetTeamMembers(int teamId)
        {
            return _gitHubService.GetTeamMembers(teamId);
        }
        public async Task<List<User>> GetUsersNotInTeam(string teamName)
        {
            var allTeams = await GetAllTeams();
            var allOrgMembers = await GetAllUsers();
            var teamId = allTeams.Single(x => x.Name.Equals(teamName, StringComparison.InvariantCultureIgnoreCase)).Id;

            var teamMembers = await GetTeamMembers(teamId);

            return allOrgMembers.Except(teamMembers, new OctokitUserEqualityComparer()).ToList();
        }
    }

    public class OctokitUserEqualityComparer : IEqualityComparer<User>
    {
        public bool Equals(User x, User y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Id.Equals(y.Id);
        }

        public int GetHashCode(User obj)
        {
            return obj.UpdatedAt.GetHashCode();
        }
    }
}
