using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Konmaripo.Web.Models;
using Microsoft.Extensions.Options;
using Octokit;
using Octokit.Internal;
using Activity = System.Diagnostics.Activity;

namespace Konmaripo.Web.Controllers
{
    public class GitHubRepo
    {
        public string Name { get; }

        public GitHubRepo(string name)
        {
            Name = name;
        }
    }

    public class GitHubSettings
    {
        public string AccessToken { get; set; }
        public string OrganizationName { get; set; }
    }

    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private GitHubClient _client;
        private GitHubSettings _ghSettings;

        public HomeController(ILogger<HomeController> logger, IOptions<GitHubSettings> gitHubSettings)
        {
            // TODO: Input checks

            _logger = logger;

            var credentials = new Credentials(token: gitHubSettings.Value.AccessToken);
            _client = new GitHubClient(new ProductHeaderValue("Konmaripo"), new InMemoryCredentialStore(credentials));
            _ghSettings = gitHubSettings.Value;
        }

        public async Task<IActionResult> Index()
        {
            // Obtain list of GitHub Repos
            // Pass through to the view
            var repos = await _client.Repository.GetAllForOrg(_ghSettings.OrganizationName);

            var resultList = repos.Select(x => new GitHubRepo(x.Name)).ToList();
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
