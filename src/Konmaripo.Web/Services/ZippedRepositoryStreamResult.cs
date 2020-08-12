using System.IO;

namespace Konmaripo.Web.Services
{
    public class ZippedRepositoryStreamResult 
    {
        public FileStream Stream { get; }
        public string FileName { get; }

        public ZippedRepositoryStreamResult(FileStream stream, string fileName)
        {
            Stream = stream;
            FileName = fileName;
        }
    }
}