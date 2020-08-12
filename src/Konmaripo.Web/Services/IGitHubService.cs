using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Konmaripo.Web.Models;
using Octokit;

namespace Konmaripo.Web.Services
{
    public interface IGitHubService
    {
        Task<List<GitHubRepo>> GetRepositoriesForOrganizationAsync();
        Task<ExtendedRepoInformation> GetExtendedRepoInformationFor(long repoId);
        Task CreateArchiveIssueInRepo(long repoId, string currentUser);
        Task ArchiveRepository(long repoId, string repoName);
        Task<RepoQuota> GetRepoQuotaForOrg();
        Task<ZippedRepositoryStreamResult> ZippedRepositoryStreamAsync(string repoName);
        int RemainingAPIRequests();
        Task CreateIssueInRepo(NewIssue issue, long repoId);
        DateTimeOffset APITokenResetTime();
        Task DeleteRepository(long repoId);
    }
}