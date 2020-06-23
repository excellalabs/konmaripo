using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Konmaripo.Web.Models;
using Octokit;
using Serilog;

namespace Konmaripo.Web.Services
{
    public class MassIssueCreator : IMassIssueCreator
    {
        private readonly System.Threading.SemaphoreSlim _batcher = new System.Threading.SemaphoreSlim(10, 10);
        private readonly IGitHubService _gitHubService;
        private readonly ILogger _logger;

        public MassIssueCreator(IGitHubService gitHubService, ILogger logger)
        {
            _gitHubService = gitHubService;
            _logger = logger;
        }

        public List<Task> CreateIssue(NewIssue issue, List<GitHubRepo> repoList)
        {
            _logger.Information("Queuing issues for {RepoCount} repositories", repoList.Count);
            var taskList = new List<Task>();

            repoList.ForEach(x=> taskList.Add(CreateIssue(issue, x.Id)));

            _logger.Information("Added tasks");

            return taskList;
        }
        private async Task CreateIssue(NewIssue issue, long repoId)
        {
            await _batcher.WaitAsync();

            try
            {
                await _gitHubService.CreateIssueInRepo(issue, repoId);
            }
            catch (Exception e)
            {
                _logger.Error(e, "An error occurred while creating an issue in repoId {RepoId}", repoId);
            }
            finally
            {
                _batcher.Release();
            }
        }
    }
}