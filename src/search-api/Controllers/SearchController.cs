using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Monitoring;
using Searc.SearchApi.Models;
using Searc.SearchApi.Services;
using Serilog;

namespace Searc.SearchApi.Controllers;

[ApiController]
[Route("api/v1/search")]
public class SearchController(ISearchService service) : Controller
{

    [HttpGet]
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
}
