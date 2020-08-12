using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Konmaripo.Web.Models;
using Konmaripo.Web.Services;
using Microsoft.Net.Http.Headers;
using Serilog;

namespace Konmaripo.Web.Controllers
{
    [Authorize]
    public class SunsettingController : Controller
    {
        private readonly ILogger _logger;
        private readonly IGitHubService _gitHubService;

        public SunsettingController(ILogger logger, IGitHubService gitHubService)
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
            _logger.Information("Archiving Repo ID {RepoId}", repoId);
            var currentUser = this.User.Identity.Name;
            _logger.Information("User is {UserId}", currentUser);

            await _gitHubService.CreateArchiveIssueInRepo(repoId, currentUser);
            await _gitHubService.ArchiveRepository(repoId, repoName);

            return View("ArchiveSuccess");
        }
        public async Task<IActionResult> DownloadRepo(long repoId, string repoName)
        {
            _logger.Information("Generating zip file of {RepoName} (id {RepoId})", repoName, repoId);
            var zippedRepositoryStream = await _gitHubService.ZippedRepositoryStreamAsync(repoName);
            _logger.Information("Stream generated for {RepoName} -- returning", repoName, repoId);
            return File(zippedRepositoryStream, "application/zip", $"{repoName}.zip");
        }

    }
}
