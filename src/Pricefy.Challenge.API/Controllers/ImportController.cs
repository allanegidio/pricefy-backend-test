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
using Pricefy.Challenge.Domain.Entities;

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
}
