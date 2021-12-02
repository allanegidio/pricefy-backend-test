using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using Pricefy.Challenge.Domain.Entities;

namespace Pricefy.Challenge.Infra.Services
{
    public class TsvService : ITsvService
    {
        private readonly CsvConfiguration _csvConfiguration;

        public TsvService(CsvConfiguration csvConfiguration)
        {
            _csvConfiguration = csvConfiguration;
        }

        public List<T> ReadFile<T>(StreamReader streamReader)
        {
            using (streamReader)
            using (var csv = new CsvReader(streamReader, _csvConfiguration))
                return csv.GetRecords<T>().ToList();
        }

        public void SplitTsvFile(string inputPath, string outputPath, string fileName, int rowsPerFile = 1000, int initialId = 1)
        {
            using (var reader = new StreamReader(inputPath))
            using (var csv = new CsvReader(reader, _csvConfiguration))
            {
                var titles = csv.GetRecords<dynamic>().ToList();

                for (var row = 0; row <= titles.Count / rowsPerFile; row++, initialId++)
                {
                    var fileRows = titles.Skip(row * rowsPerFile)
                                        .Take(rowsPerFile);

                    var outputPathWithFileName = Path.Combine(outputPath, @$"{fileName}.{initialId}.{DateTime.Now.ToString("yyyy-MM-dd")}.tsv");

                    using (var writer = new StreamWriter(outputPathWithFileName, false, System.Text.Encoding.UTF8))
                    {
                        using (var csvFile = new CsvWriter(writer, _csvConfiguration))
                        {
                            csvFile.WriteRecords(fileRows);
                        }
                    }
                }
            }
        }
    }
}