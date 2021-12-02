using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pricefy.Challenge.CLI.Constants;
using Pricefy.Challenge.Domain.Clients;
using Pricefy.Challenge.Domain.Entities;
using Pricefy.Challenge.Infra.Clients;
using Pricefy.Challenge.Infra.Services;

namespace Pricefy.Challenge.CLI
{
    class Program
    {
        public static IConfiguration _configuration;

        public static ILogger<Program> _logger;

        public static ITsvService _tsvService;

        public static void Main(string[] args)
        {
            ServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServiceProvider(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            _logger = serviceProvider.GetService<ILoggerFactory>()
                                    .CreateLogger<Program>();

            _tsvService = serviceProvider.GetService<ITsvService>();

            try
            {
                MainAsync(args).Wait();
            }
            catch
            {
            }
        }

        public static async Task MainAsync(string[] args)
        {
            Console.WriteLine("Import Title Basics from IMDB");
            Console.WriteLine("Please send the path of File that you would like to Import:");

            var inputPath = Console.ReadLine();

            Console.WriteLine("Chosen path was:" + inputPath);

            var initialId = 1;
            var outputPath = @"D:/titles/chunk";
            var fileName = @$"titles.file.{initialId}.{DateTime.Now.ToString("yyyy-MM-dd")}.tsv";

            _tsvService.SplitTsvFile(inputPath, outputPath, fileName, initialId: initialId);
        }

        public static void ConfigureServiceProvider(IServiceCollection serviceCollection)
        {
            // Add Services
            serviceCollection.AddScoped<ITsvService, TsvService>();
            serviceCollection.AddScoped<App>();

            // Add Log
            serviceCollection.AddSingleton(LoggerFactory.Create(builder => builder.AddConsole()));
            serviceCollection.AddLogging();


            serviceCollection.AddHttpClient<IImportClient, ImportClient>(PricefyConstants.ImportAPI_NAME,
                                client => client.BaseAddress = new Uri(_configuration[PricefyConstants.ImportAPI_URL]));

            // Add appsettings
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();
        }
    }
}