namespace Konmaripo.Web.Models
{
    public class RepoQuota
    {
        public long PrivateRepoLimit { get; }
        public int PrivateRepoCount { get; }

        public RepoQuota(long privateRepoLimit, int privateRepoCount)
        {
            PrivateRepoLimit = privateRepoLimit;
            PrivateRepoCount = privateRepoCount;
        }
    }
}