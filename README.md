## GitHub Governance Tool

* .NET Core
* Single exe
* Octokit .NET (with a GitHub Access token)

## Features

 * Create and pin an issue
 * Archive the repository
 * Clone a github repository
 * Zip up a repository
 * Delete repository?
 * Accept a list of repositories

## Usage

`gh-gov archive --org excellaco --repos repo1,repo2,repo3`

 * Confirm
 * Access token via environment variables / secrets
 * Create issue
 * Pin the issue
 * Archive the repo(s)
 * gh-gov delete --org --repos repo1,repo2,repo3 --archive-location
 * Confirm
 * Clone repo
 * Zip the repo 
 * Copy repo to known location
 * Delete the repo

## Adjacent Subjects

* Automated testing
* DevOps builds  (PRs)
* Tag releases
