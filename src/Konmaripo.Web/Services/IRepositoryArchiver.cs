namespace Konmaripo.Web.Services
{
    public interface IRepositoryArchiver
    {
        RepositoryPath CloneRepositoryWithTagsAndBranches(string repoName);
    }
}