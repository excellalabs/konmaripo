using System.Collections.Generic;
using System.Threading.Tasks;
using Konmaripo.Web.Models;
using Octokit;

namespace Konmaripo.Web.Services
{
    public interface IMassIssueCreator
    {
        List<Task> CreateIssue(NewIssue issue, List<GitHubRepo> repoList);
    }
}