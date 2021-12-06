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
    [Route("[controller]/[action]")]
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
        public async Task<IActionResult> ImportFile(IFormFile formFile)
        {
            try
            {
                if (formFile == null || formFile.Length < 0)
                    return BadRequest(new { success = false, message = "File null or less than 0 bytes." });

                var isImported = await _titleRepository.IsImported(formFile.FileName);

                if (isImported)
                    return Ok(new { success = isImported, message = $"TSV File {formFile.FileName} is already imported." });

                var titles = _tsvService.ReadFile<Title>(formFile.OpenReadStream().ToStreamReader());

                if (titles == null || titles.Count == 0)
                    return Ok(new { success = false, message = "An error occoured when tried to read TSV file." });

                var result = await _titleRepository.BulkInsert(formFile.FileName, titles);

                if (!result)
                    return Ok(new { success = false, message = "Import TSV File wasn't successful" });

                return Ok(new { success = true, message = "Import TSV File was successful" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occoured when import file. Message is: {ex.Message}");
                return StatusCode(500, new { message = "An error occoured when import file." });
            }
        }

        [HttpGet("{fileName}")]
        public async Task<IActionResult> IsImported(string fileName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileName))
                    return BadRequest(new { success = false, message = "File name is null, empty or whitespace" });

                var isImported = await _titleRepository.IsImported(fileName);

                if (isImported)
                    return Ok(new { success = isImported, message = $"TSV File {fileName} is already imported." });

                return Ok(new { success = true, message = "Import TSV File was successful" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occoured when check if file is already imported. Message is: {ex.Message}");
                return StatusCode(500, new { message = "An error occoured when check if file is already imported" });
            }
        }

        [HttpGet()]
        public async Task<IActionResult> GetAllFilesImported()
        {
            try
            {
                var files = await _titleRepository.GetAllFilesImported();

                if (files == null || files.Count <= 0)
                    return Ok(new { success = true, message = $"There isn't file imported yet." });

                return Ok(new { success = true, files = files, message = "Get all TSV File imported was successful" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occoured when tried list all imported files. Message is: {ex.Message}");
                return StatusCode(500, new { message = "An error occoured when tried list all imported files" });
            }
        }
    }
}
