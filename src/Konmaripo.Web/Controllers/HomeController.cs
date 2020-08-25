using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Konmaripo.Web.Models;
using Konmaripo.Web.Services;
using Microsoft.Extensions.Options;
using Serilog;
using Activity = System.Diagnostics.Activity;

namespace Konmaripo.Web.Controllers
{
    public class HomeIndexViewModel
    {
        public RepoQuota RepoQuota { get; }
        public string OrgName { get; }

        public HomeIndexViewModel(RepoQuota repoQuota, string orgName)
        {
            RepoQuota = repoQuota;
            OrgName = orgName;
        }
    }

    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger _logger;
        private readonly IGitHubService _gitHubService;
        private readonly GitHubSettings _gitHubSettings;

        public HomeController(ILogger logger, IGitHubService gitHubService, IOptions<GitHubSettings> gitHubSettings)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _gitHubService = gitHubService ?? throw new ArgumentNullException(nameof(gitHubService));
            _gitHubSettings = gitHubSettings?.Value ?? throw new ArgumentNullException(nameof(gitHubSettings));
        }

        public async Task<IActionResult> Index()
        {
            var repoQuota = await _gitHubService.GetRepoQuotaForOrg();
            var orgName = _gitHubSettings.OrganizationName;
            var vm = new HomeIndexViewModel(repoQuota, orgName);
            return View(vm);
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
