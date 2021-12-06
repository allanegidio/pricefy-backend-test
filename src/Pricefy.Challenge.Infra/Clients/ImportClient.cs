using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Pricefy.Challenge.Domain.Clients;
using Pricefy.Challenge.Domain.Responses;

namespace Pricefy.Challenge.Infra.Clients
{
    public class ImportClient : IImportClient
    {
        private readonly ILogger<ImportClient> _logger;
        private readonly HttpClient _httpClient;
        public ImportClient(HttpClient httpClient, ILogger<ImportClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IsImportedResponse> IsImported(string file)
        {
            var res = await _httpClient.GetAsync($"/IsImported/{file}");
            var responseContent = await res.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<IsImportedResponse>(responseContent);

            if (!res.IsSuccessStatusCode)
            {
                _logger.LogError(result.Message);
                return result;
            }

            _logger.LogInformation(result.Message);
            return result;
        }

        public async Task<ImportTsvFileResponse> SendTsvFile(byte[] tsv, string name, string fileName)
        {
            var content = new MultipartFormDataContent();

            content.Add(new StreamContent(new MemoryStream(tsv)), name, fileName);

            var res = await _httpClient.PostAsync("/ImportFile", content);

            var responseContent = await res.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ImportTsvFileResponse>(responseContent);

            if (!res.IsSuccessStatusCode)
            {
                _logger.LogError($"An error occoured when tried to send {fileName}. Statuscode is: {res.StatusCode} and content is: {responseContent}");
                return result;
            }

            _logger.LogInformation($"{fileName} imported with success");
            return result;
        }
    }
}