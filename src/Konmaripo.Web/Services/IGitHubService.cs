using System.Collections.Generic;
using System.Threading.Tasks;
using Konmaripo.Web.Models;

namespace Konmaripo.Web.Services
{
    public interface IGitHubService
    {
        Task<List<GitHubRepo>> GetRepositoriesForOrganizationAsync();
        Task<ExtendedRepoInformation> GetExtendedRepoInformationFor(long repoId);
    }
}