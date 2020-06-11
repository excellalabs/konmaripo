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
        public int ViewsInTheLast14Days { get; }
        public int CommitActivityInTheLast4Weeks { get; }

        public ExtendedRepoInformation(long repoId, int viewsInTheLast14Days, int commitActivityInTheLast4Weeks)
        {
            RepoId = repoId;
            ViewsInTheLast14Days = viewsInTheLast14Days;
            CommitActivityInTheLast4Weeks = commitActivityInTheLast4Weeks;
        }
    }
}
