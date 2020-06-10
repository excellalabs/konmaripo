using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Konmaripo.Web.Models
{
    public class ExtendedRepoInformation
    {
        public long RepoId { get; }
        public int NumberOfWatchers { get; }
        public int ViewsInTheLast14Days { get; }

        public ExtendedRepoInformation(long repoId, int numberOfWatchers, int viewsInTheLast14Days)
        {
            RepoId = repoId;
            NumberOfWatchers = numberOfWatchers;
            ViewsInTheLast14Days = viewsInTheLast14Days;
        }
    }
}
