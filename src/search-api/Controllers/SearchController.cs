using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Searc.SearchApi.Models;
using Searc.SearchApi.Services;

namespace Searc.SearchApi.Controllers;

[ApiController]
[Route("api/search/v1")]
public class SearchController(ISearchService service) : Controller
{

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FileDetailsDTO>>> Search([Required] string query)
    {
        using var activity = Monitoring.MonitoringService.ActivitySource.StartActivity("SearchController.Search");
        
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