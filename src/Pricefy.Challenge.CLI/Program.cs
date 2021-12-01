using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;

namespace Pricefy.Challenge.CLI
{
    public class Title
    {
        [Name("tconst")]
        public string Id { get; set; }

        [Name("titleType")]
        public string Type { get; set; }

        [Name("primaryTitle")]
        public string PrimaryTitle { get; set; }

        [Name("originalTitle")]
        public string OriginalTitle { get; set; }

        [Name("isAdult")]
        public bool IsAdult { get; set; }

        [Name("startYear")]
        public string StartYear { get; set; }

        [Name("endYear")]
        public string EndYear { get; set; }

        [Name("runtimeMinutes")]
        public string RuntimeMinutes { get; set; }

        [Name("genres")]
        public string Genres { get; set; }
    }

    class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Import Title Basics from IMDB");
            Console.WriteLine("Please send the path of File that you would like to Import:");

            var path = Console.ReadLine();

            Console.WriteLine("Chosen path was:" + path);

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = "\t",
                Mode = CsvMode.NoEscape
            };

            using (var reader = new StreamReader(@"D:/titles/data.tsv"))
            using (var csv = new CsvReader(reader, config))
            {
                int id = 1;
                int totalRowsFile = 1000;
                var titles = csv.GetRecords<dynamic>().ToList();

                for (var row = 0; row <= titles.Count / totalRowsFile; row++, id++)
                {
                    var fileRows = titles.Skip(row * totalRowsFile)
                                        .Take(totalRowsFile);

                    var outputPath = Path.Combine(@"D:/titles/chunk", @$"titles.file.{id}.{DateTime.Now.ToString("yyyy-MM-dd")}.tsv");

                    using (var writer = new StreamWriter(outputPath, false, System.Text.Encoding.UTF8))
                    {
                        using (var csvFile = new CsvWriter(writer, config))
                        {
                            csvFile.WriteRecords(fileRows);
                        }
                    }
                }
            }
        }
    }
}