using System.IO;
using System.Threading.Tasks;
using Pricefy.Challenge.Domain.Responses;

namespace Pricefy.Challenge.Domain.Clients
{
    public interface IImportClient
    {
        Task<ImportTsvFileResponse> SendTsvFile(byte[] tsv, string name, string fileName);
        Task<IsImportedResponse> IsImported(string file);
    }
}