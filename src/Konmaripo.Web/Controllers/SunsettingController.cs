using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Konmaripo.Web.Models;
using Konmaripo.Web.Services;

namespace Konmaripo.Web.Controllers
{
    [Authorize]
    public class SunsettingController : Controller
    {
        private readonly ILogger<SunsettingController> _logger;
        private readonly IGitHubService _gitHubService;

        public SunsettingController(ILogger<SunsettingController> logger, IGitHubService gitHubService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _gitHubService = gitHubService ?? throw new ArgumentNullException(nameof(gitHubService));
        }

        public async Task<IActionResult> Index()
        {
            // Obtain list of GitHub Repos
            // Pass through to the view
            var repos = await _gitHubService.GetRepositoriesForOrganizationAsync();

            _logger.LogInformation("Returning {RepoCount} repositories", repos.Count);

            return View(repos);

        }

        public async Task<IActionResult> RepositoryInfo(long repoId)
        {
            var repoInfo = await _gitHubService.GetRepositoriesForOrganizationAsync();
            var extendedInfo = await _gitHubService.GetExtendedRepoInformationFor(repoId);

            var matchingRepo = repoInfo.Single(x => x.Id == repoId);


            var vm = new IndividualRepoViewModel(matchingRepo, extendedInfo);

            return View("Repo", vm);
        }

        public async Task<IActionResult> ArchiveRepo(long repoId, string repoName)
        {
            _logger.LogInformation("Archiving Repo ID {RepoId}", repoId);
            var currentUser = this.User.Identity.Name;
            _logger.LogInformation("User is {UserId}", currentUser);

            await _gitHubService.CreateArchiveIssueInRepo(repoId, currentUser);
            await _gitHubService.ArchiveRepository(repoId, repoName);

            return View("ArchiveSuccess");
        }

    }
}
