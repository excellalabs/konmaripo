using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Konmaripo.Web.Models;
using Konmaripo.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Konmaripo.Web.Controllers
{
    public class GitHubRepoEqualityComparer : IEqualityComparer<GitHubRepo>
    {
        public bool Equals(GitHubRepo x, GitHubRepo y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Id == y.Id;
        }

        public int GetHashCode(GitHubRepo obj)
        {
            return obj.Id.GetHashCode();
        }
    }

    [Authorize]
    public class OrgWideVisibilityController : Controller
    {
        private readonly OrgWideVisibilitySettings _settings;
        private readonly IGitHubService _gitHubService;

        public OrgWideVisibilityController(IOptions<OrgWideVisibilitySettings> visibilitySettings, IGitHubService gitHubService)
        {
            if (visibilitySettings == null){throw new ArgumentNullException(nameof(visibilitySettings));}
            if (string.IsNullOrWhiteSpace(visibilitySettings.Value.AllOrgMembersGroupName)){throw new ArgumentNullException(nameof(visibilitySettings.Value.AllOrgMembersGroupName));}

            _settings = visibilitySettings.Value;
            _gitHubService = gitHubService ?? throw new ArgumentNullException(nameof(gitHubService));
        }
        public async Task<IActionResult> Index()
        {
            var teamExists = await _gitHubService.TeamExists(_settings.AllOrgMembersGroupName);

            var vm = new OrgWideVisibilityIndexVM(_settings.AllOrgMembersGroupName, teamExists);
            return View(vm);
        }

        public async Task<IActionResult> AddOrgMembersList(List<string> loginsToAdd)
        {
            await _gitHubService.AddMembersToTeam(_settings.AllOrgMembersGroupName, loginsToAdd);

            return RedirectToAction("AddOrgMembers");
        }

        public async Task<IActionResult> AddOrgMembers()
        {
            var usersNotInTeam = await _gitHubService.GetUsersNotInTeam(_settings.AllOrgMembersGroupName);

            var userLogins = usersNotInTeam.Select(x => x.Login).ToList();

            return View(userLogins);
        }

        public async Task<IActionResult> CreateOrgWideTeam()
        {
            await _gitHubService.CreateTeam(_settings.AllOrgMembersGroupName, _settings.AllOrgMembersGroupDescription);

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> RepositoryReconciliation()
        {
            var comparer = new GitHubRepoEqualityComparer();

            var allRepos = await _gitHubService.GetRepositoriesForOrganizationAsync();
            var reposWithExemptionTopic = await _gitHubService.GetRepositoriesWithTopic(_settings.ExemptionTagName);
            var reposThatAlreadyHaveTeamAccess = await _gitHubService.GetRepositoriesForTeam(_settings.AllOrgMembersGroupName);


            var reposToAddTeamTo = 
                allRepos
                .Except(reposThatAlreadyHaveTeamAccess, comparer)
                .Except(reposWithExemptionTopic, comparer).ToList();

            var reposToRemoveTeamFrom = 
                reposThatAlreadyHaveTeamAccess
                    .Intersect(reposWithExemptionTopic, comparer)
                    .ToList();

            var vm = new RepositoryReconciliationViewModel(_settings.ExemptionTagName, _settings.AllOrgMembersGroupName, reposToAddTeamTo, reposToRemoveTeamFrom);
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> RepositoryReconciliation (RepositoryReconciliationViewModel vm)
        {
            _gitHubService.AddAllOrgTeamToRepos(vm.RepositoriesToAddAccessTo);
            _gitHubService.RemoveAllOrgTeamFromRepos(vm.RepositoriesToRemoveAccessFrom);
        }
    }

    public class RepositoryReconciliationViewModel
    {
        public string ExemptionTagName { get; }
        public string AllOrgMemberTeamName { get; }
        public List<GitHubRepo> RepositoriesToAddAccessTo { get; }
        public List<GitHubRepo> RepositoriesToRemoveAccessFrom { get; }
        
        public RepositoryReconciliationViewModel(string exemptionTagName, string allOrgMemberTeamName, List<GitHubRepo> reposToAdd, List<GitHubRepo> reposToRemove)
        {
            ExemptionTagName = exemptionTagName;
            AllOrgMemberTeamName = allOrgMemberTeamName;
            RepositoriesToAddAccessTo = reposToAdd;
            RepositoriesToRemoveAccessFrom = reposToRemove;
        }
    }
    public class OrgWideVisibilityIndexVM
    {
        public string OrgWideTeamName { get; }
        public bool TeamExists { get; }

        public OrgWideVisibilityIndexVM(string orgWideTeamName, bool teamExists)
        {
            OrgWideTeamName = orgWideTeamName;
            TeamExists = teamExists;
        }


    }

    public static class ArrayExtensions
    {
        /// <summary>
        /// Splits an array into several smaller arrays.
        /// </summary>
        /// <typeparam name="T">The type of the array.</typeparam>
        /// <param name="array">The array to split.</param>
        /// <param name="size">The size of the smaller arrays.</param>
        /// <returns>An array containing smaller arrays.</returns>
        /// <seealso cref="https://www.jerriepelser.com/blog/approaches-when-rendering-list-using-bootstrap-grid-system/"/>
        /// <seealso cref="https://stackoverflow.com/questions/18986129/c-splitting-an-array-into-n-parts/18987605"/>
        public static IEnumerable<IEnumerable<T>> Split<T>(this T[] array, int size)
        {
            for (var i = 0; i < (float)array.Length / size; i++)
            {
                yield return array.Skip(i * size).Take(size);
            }
        }
    }

}
