using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Pricefy.Challenge.Domain.Clients;

namespace Pricefy.Challenge.Infra.Clients
{
    public class ImportClient : IImportClient
    {
        private readonly ILogger<ImportClient> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        public ImportClient(IHttpClientFactory httpClientFactory, ILogger<ImportClient> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        // public async Task<HttpResponseMessage> SendTSVFile(string pathUrl)
        // {

        // }
    }
}