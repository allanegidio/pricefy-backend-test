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
using Pricefy.Challenge.API.Extensions;
using Pricefy.Challenge.Domain.Entities;
using Pricefy.Challenge.Domain.Repositories;

namespace Pricefy.Challenge.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImportController : ControllerBase
    {
        private readonly ILogger<ImportController> _logger;

        private readonly ITsvService _tsvService;

        private readonly ITitleRepository _titleRepository;

        public ImportController(ILogger<ImportController> logger, ITsvService tsvService, ITitleRepository titleRepository)
        {
            _logger = logger;
            _tsvService = tsvService;
            _titleRepository = titleRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Import(IFormFile formFile)
        {
            try
            {
                if (formFile == null || formFile.Length == 0)
                    return BadRequest();

                var titles = _tsvService.ReadFile<Title>(formFile.OpenReadStream().ToStreamReader());

                _titleRepository.BulkInsert(titles);


                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occoured when import file." });
            }
        }
    }
}
