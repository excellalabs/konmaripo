﻿using System.Collections.Generic;
using System.IO;
using Konmaripo.Web.Models;
using LibGit2Sharp;
using Microsoft.Extensions.Options;

namespace Konmaripo.Web.Services
{
    public class RepositoryArchiver : IRepositoryArchiver
    {
        private readonly string _githubOrgName;
        private readonly string _accessToken;
        private const string START_PATH = "./Data"; // TODO: Extract to config

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
                CredentialsProvider = (_, _, _) => creds,
                Prune = false
            };

            var options = new CloneOptions
            {
                Checkout = true,
                IsBare = false,
                RecurseSubmodules = true,
                CredentialsProvider = (_, _, _) => creds,
                FetchOptions = fetchOptions
            };

            var clonePath = Path.Combine(START_PATH, repoName);

            // TODO: Make async
            // ReSharper disable once RedundantNameQualifier -- so we can quickly tell between Octokit and Libgit2sharp
            var pathToRepoGitFile = LibGit2Sharp.Repository.Clone(url, clonePath, options);

            // This ensures all branches and tags get fetched as well.
            // ReSharper disable once RedundantNameQualifier -- so we can quickly tell between Octokit and Libgit2sharp
            using (var repo = new LibGit2Sharp.Repository(pathToRepoGitFile))
            {
                repo.Network.Fetch("origin", new List<string>(){ "+refs/*:refs/*" }, fetchOptions);
            }
            
            var pathToFullRepo = pathToRepoGitFile.Replace(".git/", ""); // Directory.GetParent didn't work for this, maybe due to the period in the directory name.
            return new RepositoryPath(pathToFullRepo);
        }
    }
}