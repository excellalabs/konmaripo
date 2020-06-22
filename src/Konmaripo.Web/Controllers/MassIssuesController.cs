using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Konmaripo.Web.Models;
using Konmaripo.Web.Services;
using Serilog;

namespace Konmaripo.Web.Controllers
{
    [Authorize]
    public class MassIssuesController : Controller
    {
        private readonly ILogger _logger;
        private readonly IGitHubService _gitHubService;

        public MassIssuesController(ILogger logger, IGitHubService gitHubService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _gitHubService = gitHubService ?? throw new ArgumentNullException(nameof(gitHubService));
        }

        public async Task<IActionResult> Index()
        {
            // Obtain list of GitHub Repos
            // Pass through to the view
            var repos = await _gitHubService.GetRepositoriesForOrganizationAsync();

            _logger.Information("Returning {RepoCount} repositories", repos.Count);

            return View(repos);

        }
    }
}
