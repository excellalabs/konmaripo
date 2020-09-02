using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Konmaripo.Web.Models;
using Konmaripo.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Konmaripo.Web.Controllers
{
    public class OrgWideVisibilityController : Controller
    {
        private OrgWideVisibilitySettings _settings;
        private IGitHubService _gitHubService;

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

        public async Task<IActionResult> AddOrgMembers()
        {
            var usersNotInTeam = await _gitHubService.GetUsersNotInTeam(_settings.AllOrgMembersGroupName);

            return View(usersNotInTeam);
        }

        public async Task<IActionResult> CreateOrgWideTeam()
        {
            await _gitHubService.CreateTeam(_settings.AllOrgMembersGroupName, _settings.AllOrgMembersGroupDescription);

            return RedirectToAction("Index");
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
