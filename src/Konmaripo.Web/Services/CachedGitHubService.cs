using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Konmaripo.Web.Services
{
    public class CachedGitHubService
    {
        private IGitHubService _gitHubService;
        public CachedGitHubService(IGitHubService gitHubService)
        {
            _gitHubService = gitHubService ?? throw new ArgumentNullException(nameof(gitHubService));
        }
    }
}
