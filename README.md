## Konmaripo: Konmari your GitHub Org and Its Repositories

**NOTE:** This tool is in early and experimental stages; it is not currently recommended for use.

This tool is a simple web app to allow you to look at all of the repositories in your GitHub organization and see if it's time to archive them, with automated steps to help you sunset the repository.

## Ingredients
This tool uses: 

* .NET Core MVC web app
* Azure AD Integration
* Azure DevOps
* Docker containers
* Octokit .NET (with a GitHub Access token)

## Planned Feature Roadmap

* [x] Delegated access via Azure AD. Admins can choose who can triage these repositories
* [x] See applications and statistics to enable you to make an informed decision
* [ ] Archival: Create and pin an issue
* [ ] Archival: Archive the repository
* [ ] Removal: Clone a github repository
* [ ] Removal: Zip up a repository
* [ ] Removal: Delete repository?
* [ ] Enforce a waiting period between archival and removal
