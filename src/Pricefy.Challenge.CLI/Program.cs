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

        public static IImportClient _importClient;

        public static void Main(string[] args)
        {
            ServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServiceProvider(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            _logger = serviceProvider.GetService<ILoggerFactory>()
                                    .CreateLogger<Program>();

            _tsvService = serviceProvider.GetService<ITsvService>();

            _importClient = serviceProvider.GetService<IImportClient>();

            try
            {
                MainAsync(args).Wait();
                _logger.LogInformation("Import File Finished");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Occured an error when tryed import tsv file: {ex.Message}");
            }
        }

        public static async Task MainAsync(string[] args)
        {
            Console.WriteLine("Import Title Basics from IMDB");

            Console.WriteLine("What would you like? 1 - Import File or 2 - List all imported files.");
            Console.WriteLine("Press 1 - Import File");
            Console.WriteLine("Press 2 - List all imported files.");

            int inputDecision;
            while (!int.TryParse(Console.ReadLine(), out inputDecision))
            {
                Console.Clear();
                Console.WriteLine("You entered an invalid number");
                Console.Write("enter number of conversations ");
            }

            switch (inputDecision)
            {
                case 1:
                    await ImportTsvFile();
                    break;

                case 2:
                    await ListAllFilesImported();
                    break;

                default:
                    Console.WriteLine("There isn't an action for this number.");
                    break;
            }
        }

        private static Task ListAllFilesImported()
        {
            throw new NotImplementedException();
        }

        private static async Task ImportTsvFile()
        {

            Console.WriteLine("Please send the path of File that you would like to Import:");
            var inputPath = Console.ReadLine();

            Console.WriteLine("Chosen path was:" + inputPath);

            var outputPath = @"D:/titles/chunk/";
            var fileName = "titles.file";

            _tsvService.SplitTsvFile(inputPath, outputPath, fileName);

            var files = Directory.GetFiles(@"D:/titles/chunk/");

            foreach (var file in files)
            {
                try
                {
                    string fileNameReq = Path.GetFileName(file);
                    var isImported = await _importClient.IsImported(file);

                    if (isImported.IsImported)
                    {
                        _logger.LogError(isImported.Message);
                        Console.WriteLine(isImported.Message);
                        continue;
                    }

                    var bytes = File.ReadAllBytes(file);
                    await _importClient.SendTsvFile(bytes, "formFile", fileNameReq);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Occured an error when tryed import tsv file: {ex.Message}");
                }
            }
        }

        private static void ConfigureServiceProvider(IServiceCollection serviceCollection)
        {
            // Add appsettings
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();

            serviceCollection.AddScoped<ITsvService>(service => new TsvService(new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = "\t", Mode = CsvMode.NoEscape }));

            serviceCollection.AddSingleton(LoggerFactory.Create(builder => builder.AddConsole()));
            serviceCollection.AddLogging();


            serviceCollection.AddHttpClient<IImportClient, ImportClient>(PricefyConstants.ImportAPI_NAME,
                                client => client.BaseAddress = new Uri(_configuration[PricefyConstants.ImportAPI_URL]));

        }
    }
}