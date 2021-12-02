using System.IO;
using System.Threading.Tasks;

namespace Pricefy.Challenge.Domain.Clients
{
    public interface IImportClient
    {
        Task SendTSVFile(byte[] tsv, string name, string fileName);
    }
}