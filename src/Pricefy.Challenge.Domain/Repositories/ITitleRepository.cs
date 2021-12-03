using System.Collections.Generic;
using System.Threading.Tasks;
using Pricefy.Challenge.Domain.Entities;

namespace Pricefy.Challenge.Domain.Repositories
{
    public interface ITitleRepository
    {
        void BulkInsert(List<Title> titles);

        Task<bool> IsImported(string fileName);
    }
}