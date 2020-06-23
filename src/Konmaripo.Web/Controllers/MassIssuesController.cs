using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Konmaripo.Web.Services;
using Octokit;
using Serilog;

namespace Konmaripo.Web.Controllers
{
    public class MassIssue
    {
        [Required]
        public string Subject { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string Body { get; set; }

        [Display(Name = "Pin Issue?")]
        public bool ShouldBePinned { get; set; }

        // ReSharper disable once UnusedMember.Global
        public MassIssue()
        {
            // this is here because the model binding uses it
        }
        public MassIssue(string subject, string body, bool shouldBePinned)
        {
            Subject = subject;
            Body = body;
            ShouldBePinned = shouldBePinned;
        }
    }
    public class MassIssueViewModel
    {
        public MassIssue MassIssue { get; set; }
        public int NonArchivedRepos { get; set; }
        public int RemainingAPIRequests { get; set; }

        // ReSharper disable once UnusedMember.Global
        public MassIssueViewModel()
        {
            // This is here because the model binding uses it
        }
        public MassIssueViewModel(MassIssue massIssue, int nonArchivedRepos, int remainingApiRequests)
        {
            MassIssue = massIssue;
            NonArchivedRepos = nonArchivedRepos;
            RemainingAPIRequests = remainingApiRequests;
        }
    }

    [Authorize]
    public class MassIssuesController : Controller
    {
        private readonly ILogger _logger;
        private readonly IGitHubService _gitHubService;
        private readonly IMassIssueCreator _massIssueCreator;

        public MassIssuesController(ILogger logger, IGitHubService gitHubService, IMassIssueCreator massIssueCreator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _gitHubService = gitHubService ?? throw new ArgumentNullException(nameof(gitHubService));
            _massIssueCreator = massIssueCreator ?? throw new ArgumentNullException(nameof(massIssueCreator));
        }

        public async Task<IActionResult> Index()
        {
            var remainingRequests = _gitHubService.RemainingAPIRequests();
            var nonArchivedRepos = await NonArchivedReposCount();
            var issue = new MassIssue(string.Empty, string.Empty, false);

            var vm = new MassIssueViewModel(issue, nonArchivedRepos, remainingRequests);

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Index(MassIssueViewModel vm)
        {
            var nonArchivedReposCount = await NonArchivedReposCount();
            if (!ModelState.IsValid)
            {
                _logger.Warning("Mass issue model is invalid; returning validation error messages.");
                vm.MassIssue.Subject = vm.MassIssue.Subject;
                vm.MassIssue.Body = vm.MassIssue.Body;
                vm.NonArchivedRepos = nonArchivedReposCount;
                vm.RemainingAPIRequests = _gitHubService.RemainingAPIRequests();
                return View(vm);
            }
            else
            {
                var currentUser = this.User.Identity.Name;
                var newIssue = new NewIssue(vm.MassIssue.Subject);
                newIssue.Body = @$"{vm.MassIssue.Body}

----

Created by {currentUser} using the Konmaripo tool";

                try
                {
                    var repos = await _gitHubService.GetRepositoriesForOrganizationAsync();
                    var nonArchivedRepos = repos.Where(x => !x.IsArchived).ToList();
                    await Task.WhenAll(_massIssueCreator.CreateIssue(newIssue, nonArchivedRepos));
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "An error occurred while creating the mass issues.");
                }

                return View("IssueSuccess");
            }


        }

        private async Task<int> NonArchivedReposCount()
        {
            var repos = await _gitHubService.GetRepositoriesForOrganizationAsync();
            return repos.Count(x => !x.IsArchived);
        }
    }
}
