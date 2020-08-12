using System;
using System.IO;
using System.Linq;
using Konmaripo.Web.Models;
using LibGit2Sharp;
using Microsoft.Extensions.Options;

namespace Konmaripo.Web.Services
{
    public class RepositoryArchiver : IRepositoryArchiver
    {
        private readonly string _githubOrgName;
        private readonly string _accessToken;
        const string REMOTE_NAME = "origin"; // hard-coded since this will be the default when cloned from GitHub.
        const string START_PATH = "./Data"; // TODO: Extract to config

        public RepositoryArchiver(IOptions<GitHubSettings> githubSettings)
        {
            _githubOrgName = githubSettings.Value.OrganizationName;
            _accessToken = githubSettings.Value.AccessToken;
        }

        public RepositoryPath CloneRepositoryWithTagsAndBranches(string repoName)
        {
            var url = $"https://github.com/{_githubOrgName}/{repoName}.git".ToLowerInvariant();
            
            var creds = new UsernamePasswordCredentials
            {
                Username = _accessToken,
                Password = string.Empty
            };

            var fetchOptions = new FetchOptions()
            {
                TagFetchMode = TagFetchMode.All,
                CredentialsProvider = (_url, _user, _cred) => creds,
            };

            var options = new CloneOptions
            {
                Checkout = true,
                IsBare = false,
                RecurseSubmodules = true,
                // ReSharper disable InconsistentNaming
                CredentialsProvider = (_url, _user, _cred) => creds,
                FetchOptions = fetchOptions
                // ReSharper enable InconsistentNaming
            };

            //var destinationArchiveFileName = Path.Combine(START_PATH, $"{repoName}.zip");
            var clonePath = Path.Combine(START_PATH, repoName);

            // TODO: Make async
            var pathToRepoGitFile = LibGit2Sharp.Repository.Clone(url, clonePath, options);

            // This ensures all branches and tags get fetched as well.
            using (var repo = new LibGit2Sharp.Repository(pathToRepoGitFile))
            {
                var remoteBranches = repo.Branches.Where(x => x.IsRemote && !x.IsTracking).ToList();

                var nonExistingRemoteBranches = remoteBranches.Where(x =>
                {
                    var localBranchName = GenerateLocalBranchName(x);
                    return repo.Branches[localBranchName] == null;
                }).ToList();

                foreach (var remoteBranch in nonExistingRemoteBranches)
                {
                    var localBranchName = GenerateLocalBranchName(remoteBranch);

                    var localCreatedBranch = repo.CreateBranch(localBranchName, remoteBranch.Tip);
                    repo.Branches.Update(localCreatedBranch, b => b.TrackedBranch = remoteBranch.CanonicalName);
                }
                repo.Network.Fetch(REMOTE_NAME, new[] { $"+refs/heads/*:refs/remotes/origin/*" }, fetchOptions);
                var mergeOptions = new MergeOptions
                {
                    FastForwardStrategy = FastForwardStrategy.Default,
                    CommitOnSuccess = true,
                    FailOnConflict = true,
                    MergeFileFavor = MergeFileFavor.Theirs
                };
                var pullOptions = new PullOptions { FetchOptions = fetchOptions, MergeOptions = mergeOptions };
                var sig = new LibGit2Sharp.Signature("Konmaripo Tool", "konmaripo@excella.com", DateTimeOffset.UtcNow);
                Commands.Pull(repo, sig, pullOptions);
            }

            
            var pathToFullRepo = pathToRepoGitFile.Replace(".git/", ""); // Directory.GetParent didn't work for this, maybe due to the period in the directory name.
            return new RepositoryPath(pathToFullRepo);
        }

        private string GenerateLocalBranchName(Branch x)
        {
            return x.FriendlyName.Replace($"{REMOTE_NAME}/", string.Empty);
        }

    }
}