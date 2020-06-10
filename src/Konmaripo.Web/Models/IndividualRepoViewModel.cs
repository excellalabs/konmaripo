using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Konmaripo.Web.Models
{
    public class IndividualRepoViewModel
    {
        public GitHubRepo RepoInfo { get; }
        public ExtendedRepoInformation ExtendedInfo { get; }

        public IndividualRepoViewModel(GitHubRepo repoInfo, ExtendedRepoInformation extendedInfo)
        {
            RepoInfo = repoInfo;
            ExtendedInfo = extendedInfo;
        }
    }
}
