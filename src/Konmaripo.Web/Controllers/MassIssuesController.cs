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
        public string Subject { get; }

        [DataType(DataType.MultilineText)]
        public string Body { get; }

        [Display(Name = "Pin Issue?")]
        public bool ShouldBePinned { get; }

        public MassIssue(string subject, string body, bool shouldBePinned)
        {
            Subject = subject;
            Body = body;
            ShouldBePinned = shouldBePinned;
        }
    }
    public class MassIssueViewModel
    {
        public MassIssue MassIssue { get; }
        public int NonArchivedRepos { get; }
        public int RemainingAPIRequests { get; }

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
            var nonArchivedRepos = allRepos.Count(x => !x.IsArchived);
            var issue = new MassIssue(string.Empty, string.Empty, false);

            var vm = new MassIssueViewModel(issue, nonArchivedRepos, remainingRequests);

            return View(vm);

        }
    }
}
