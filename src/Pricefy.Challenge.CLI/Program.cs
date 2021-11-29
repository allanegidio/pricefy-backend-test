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
        public string Id { get; set; }

        public string Type { get; set; }

        public string PrimaryTitle { get; set; }

        public string OriginalTitle { get; set; }

        public bool IsAdult { get; set; }

        public string StartYear { get; set; }

        public string EndYear { get; set; }

        public string RuntimeMinutes { get; set; }

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
                Mode = CsvMode.NoEscape
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
                        Id = csv.GetField<string>("tconst"),
                        Type = csv.GetField<string>("titleType"),
                        PrimaryTitle = csv.GetField<string>("primaryTitle"),
                        OriginalTitle = csv.GetField<string>("originalTitle"),
                        IsAdult = csv.GetField<bool>("isAdult"),
                        StartYear = csv.GetField<string>("startYear"),
                        EndYear = csv.GetField<string>("endYear"),
                        RuntimeMinutes = csv.GetField<string>("runtimeMinutes"),
                        Genres = csv.GetField<string>("genres").Split(",").ToList()
                    });
                }
            }

            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds / 1000);
        }
    }
}