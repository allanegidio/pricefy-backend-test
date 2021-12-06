using System.Collections.Generic;
using System.Threading.Tasks;
using Pricefy.Challenge.Domain.Entities;

namespace Pricefy.Challenge.Domain.Repositories
{
    public interface ITitleRepository
    {
        Task<bool> BulkInsert(string fileName, List<Title> titles);

        Task<bool> IsImported(string fileName);

        Task<List<FileDetail>> GetAllFilesImported();

        Task SaveFileDetail(string fileName, int count);
    }
}