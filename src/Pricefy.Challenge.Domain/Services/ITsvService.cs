using System.Collections.Generic;
using System.IO;

namespace Pricefy.Challenge.Domain.Entities
{
    public interface ITsvService
    {
        List<T> ReadFile<T>(StreamReader streamReader);

        void SplitTsvFile(string inputPath, string outputPath, string fileName, int rowsPerFile = 1000, int initialId = 1);
    }
}