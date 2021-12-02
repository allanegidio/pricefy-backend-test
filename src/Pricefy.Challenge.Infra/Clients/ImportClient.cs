using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Pricefy.Challenge.Domain.Clients;

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

        public async Task SendTSVFile(byte[] tsv, string name, string fileName)
        {
            var content = new MultipartFormDataContent();

            // using (var ms = new MemoryStream())
            // {
            //     tsv.CopyTo(ms);
            //     content.Add(new StreamContent(new MemoryStream(ms.ToArray())), name, fileName);


            //     var res = await _httpClient.PostAsync("/Import", content);
            //     var result = await res.Content.ReadAsStringAsync();
            // }


            content.Add(new StreamContent(new MemoryStream(tsv)), name, fileName);


            var res = await _httpClient.PostAsync("/Import", content);
            var result = await res.Content.ReadAsStringAsync();





        }
    }
}