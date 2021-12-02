using System;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pricefy.Challenge.CLI.Constants;
using Pricefy.Challenge.Domain.Clients;
using Pricefy.Challenge.Domain.Entities;
using Pricefy.Challenge.Infra.Clients;
using Pricefy.Challenge.Infra.Services;

namespace Pricefy.Challenge.CLI
{
    class Program
    {
        public IConfiguration _configuration;

        public void ConfigureServiceProvider(IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<ITsvService, TsvService>();

            serviceCollection.AddLogging()
                            .AddHttpClient<IImportClient, ImportClient>(PricefyConstants.ImportAPI_NAME,
                                client => client.BaseAddress = new Uri(_configuration[PricefyConstants.ImportAPI_URL])
                            );
        }

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