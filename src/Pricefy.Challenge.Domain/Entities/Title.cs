using CsvHelper.Configuration.Attributes;

namespace Pricefy.Challenge.Domain.Entities
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
}