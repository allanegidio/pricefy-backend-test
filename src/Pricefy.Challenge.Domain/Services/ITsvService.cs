using System.Collections.Generic;
using System.IO;

namespace Pricefy.Challenge.Domain.Entities
{
    public interface ITsvService
    {
        List<T> ReadFile<T>(StreamReader streamReader);
    }
}