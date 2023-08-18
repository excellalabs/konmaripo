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
        Task<bool> TeamExists(string teamName);
        Task CreateTeam(string teamName, string teamDescription);
        Task<List<User>> GetUsersNotInTeam(string teamName);
        Task<IReadOnlyList<User>> GetAllUsers();
        Task<IReadOnlyList<Team>> GetAllTeams();
        Task<IReadOnlyList<User>> GetTeamMembers(int teamId);
        Task AddMembersToTeam(string teamName, List<string> loginsToAdd);
        Task AddMembersToTeam(int teamId, List<string> loginsToAdd);
        Task<List<GitHubRepo>> GetRepositoriesWithTopic(string topicName);
        Task<List<GitHubRepo>> GetRepositoriesForTeam(string teamName);
        Task AddAllOrgTeamToRepos(List<GitHubRepo> vmRepositoriesToAddAccessTo, string teamName);
        Task RemoveAllOrgTeamFromRepos(List<GitHubRepo> vmRepositoriesToRemoveAccessFrom, string teamName);
    }
}