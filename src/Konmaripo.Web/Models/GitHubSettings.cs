using System;
using System.Linq;

namespace Konmaripo.Web.Models
{
    public class GitHubSettings
    {
        public string AccessToken { get; set; }
        public string OrganizationName { get; set; }
        public string TimeZoneDisplayId { get; set; }
    }
}