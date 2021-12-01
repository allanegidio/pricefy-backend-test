using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Pricefy.Challenge.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImportController : ControllerBase
    {
        private readonly ILogger<ImportController> _logger;

        public ImportController(ILogger<ImportController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Import(IFormFile formFile)
        {
            if (formFile == null || formFile.Length == 0)
            {
                return BadRequest();
            }

            var listTitles = new List<Title>();

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = "\t",
                Mode = CsvMode.NoEscape
            };

            using (var reader = new StreamReader(formFile.OpenReadStream()))
            using (var csv = new CsvReader(reader, config))
            {
                csv.Read();
                csv.ReadHeader();

                var titles = csv.GetRecords<Title>().ToList();
            }

            return Ok();
        }
    }

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
