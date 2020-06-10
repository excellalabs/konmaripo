using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Konmaripo.Web.Models;
using Konmaripo.Web.Services;
using Activity = System.Diagnostics.Activity;

namespace Konmaripo.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IGitHubService _gitHubService;

        public HomeController(ILogger<HomeController> logger, IGitHubService gitHubService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _gitHubService = gitHubService ?? throw new ArgumentNullException(nameof(gitHubService));
        }

        public async Task<IActionResult> Index()
        {
            // Obtain list of GitHub Repos
            // Pass through to the view
            var repos = await _gitHubService.GetRepositoriesForOrganizationAsync();

            var resultList = repos.Select(x => new GitHubRepo(x.Name, x.StarCount)).ToList();
            
            _logger.LogInformation("Returning {RepoCount} repositories", resultList.Count);

            return View(resultList);

        }


        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
