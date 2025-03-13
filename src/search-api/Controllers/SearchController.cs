using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Monitoring;
using Searc.SearchApi.Models;
using Searc.SearchApi.Services;
using Serilog;
using System.IO;

namespace Searc.SearchApi.Controllers;

[ApiController]
[Route("api/v1")]
public class SearchController(ISearchService service) : Controller
{

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<FileDetailsDTO>>> Search([Required] string query)
    {
        
            Log.Logger.Information("Searching for files with query: {Query}", query);
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Search query cannot be empty");
            }

            try
            {
                var result = await service.SearchFilesAsync(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        
    }

    [HttpGet("download/{fileId}")]
    public async Task<IActionResult> DownloadFile([Required] int fileId)
    {
        Log.Logger.Information("Downloading file with ID: {FileId}", fileId);
        try
        {
            var fileData = await service.GetFileAsync(fileId);
            if (fileData == null)
                return NotFound($"File with ID {fileId} not found");

            return File(fileData, "application/octet-stream");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}
