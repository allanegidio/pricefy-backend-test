using System.Collections.Generic;
using Pricefy.Challenge.Domain.Entities;

namespace Pricefy.Challenge.Domain.Repositories
{
    public interface ITitleRepository
    {
        void BulkInsert(List<Title> titles);
    }
}