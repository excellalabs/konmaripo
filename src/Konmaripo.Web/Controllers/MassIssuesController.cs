using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Konmaripo.Web.Models;
using Konmaripo.Web.Services;
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

        public MassIssuesController(ILogger logger, IGitHubService gitHubService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _gitHubService = gitHubService ?? throw new ArgumentNullException(nameof(gitHubService));
        }

        public async Task<IActionResult> Index()
        {
            var remainingRequests = _gitHubService.RemainingAPIRequests();
            var allRepos = await _gitHubService.GetRepositoriesForOrganizationAsync();
            var nonArchivedRepos = await NonArchivedRepos();
            var issue = new MassIssue(string.Empty, string.Empty, false);

            var vm = new MassIssueViewModel(issue, nonArchivedRepos, remainingRequests);

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Index(MassIssueViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                // TODO: Log?
                vm.NonArchivedRepos = await NonArchivedRepos();
                vm.RemainingAPIRequests = _gitHubService.RemainingAPIRequests();
                return View(vm);
            }

            return await Index(); // TODO: Do something
        }

        private async Task<int> NonArchivedRepos()
        {
            var repos = await _gitHubService.GetRepositoriesForOrganizationAsync();
            return repos.Count(x => !x.IsArchived);
        }
    }
}
