## Konmaripo: KonMari your GitHub Org and Its Repositories

[![Build Status](https://dev.azure.com/excellaco/Konmaripo/_apis/build/status/excellalabs.konmaripo?branchName=main)](https://dev.azure.com/excellaco/Konmaripo/_build/latest?definitionId=8&branchName=main) 

![Deployment to Production](https://vsrm.dev.azure.com/excellaco/_apis/public/Release/badge/e5fb7cc0-7eab-46dc-9e97-aa657e4fe6d5/1/1)

**NOTE:** This tool is in early and experimental stages; it is not currently recommended for use, because we're cautious folk.

This tool is a simple web app to allow you to look at all of the repositories in your GitHub organization and see if it's time to archive them, with automated steps to help you sunset the repository.

## Current Features

* Azure AD Auth -- delegate app access only to those you trust and ensure they're authenticated.
* Submit an issue to every repository in your organization (org-wide announcements, etc.)
* View repository information in a list at a glance (stars, commits, description, key dates)
* Archive a Repository (pins an issue to the repository and archives it)
* Delete process for a repository (downloads a .zip of repository with all branches/tags pulled, allows for you to specify a URL for archival, and allows the deletion of the repository)

## Ingredients
This tool uses: 

* .NET Core MVC web app
* Azure AD Integration
* Azure DevOps for the build process
* Docker container
* Azure Web Apps for Containers (in our specific case; not required)
* [Octokit .NET](https://github.com/octokit/octokit.net) (with a GitHub Access token with admin permissions)
* [libgit2sharp](https://github.com/libgit2/libgit2sharp) for archiving and downloading repositories.

## About the Project Name
It is meant as an homage to [Marie Kondo's KonMari method](https://konmari.com/), to enable you to clean up your GitHub organization when things don't quite spark joy.

## Jump in! 

We welcome your contributions! We're currently organizing things in this way:

* A [Roadmap project](https://github.com/excellalabs/konmaripo/projects/2) that gives insight into what our next major feature push will be
* [Projects](https://github.com/excellalabs/konmaripo/projects) for each epic we're working on, so you can see a roughly prioritized backlog within a given area.

If you'd like to jump in, comment on an issue and we'll work to get you involved! Don't worry if it's daunting; we'll help you out.
