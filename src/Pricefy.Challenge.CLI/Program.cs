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
        public List<string> Genres { get; set; }
    }

    class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Import Title Basics from IMDB");
            Console.WriteLine("Please send the path of File that you would like to Import:");

            //var path = Console.ReadLine();

            //Console.WriteLine("Chosen path was:" + path);

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var listTitles = new List<Title>();

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = "\t",
                Mode = CsvMode.NoEscape,
                // HeaderValidated = null
            };

            using (var reader = new StreamReader(@"C:/titles/data.tsv"))
            using (var csv = new CsvReader(reader, config))
            {
                csv.Read();
                csv.ReadHeader();

                while (csv.Read())
                {
                    listTitles.Add(new Title()
                    {
                        Id = csv.GetField<string>("tconst")
                    });
                }
            }

            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds / 1000);
        }
    }
}