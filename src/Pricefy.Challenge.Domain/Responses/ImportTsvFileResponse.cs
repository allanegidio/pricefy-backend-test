using System.Text.Json.Serialization;

namespace Pricefy.Challenge.Domain.Responses
{
    public class ImportTsvFileResponse
    {
        [JsonPropertyName("success")]
        public string Success { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}