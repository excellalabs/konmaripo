using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Konmaripo.Web.Models;
using Konmaripo.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Options;
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

}
