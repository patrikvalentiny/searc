using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using EasyNetQ;
using Microsoft.AspNetCore.Mvc;
using Searc.SearchApi.Models;
using Searc.SearchApi.Services;

namespace Searc.SearchApi.Controllers;

[ApiController]
[Route("api/search/v1")]
public class SearchController(ISearchService service, IBus bus) : Controller
{

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FileDetailsDTO>>> Search([Required] string query)
    {
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

    [HttpPost]
    public async Task<ActionResult<FileDetailsDTO>> Add([FromBody] FileContent msg)
    {
        bus.PubSub.Publish(msg, "file-content");
        if (msg == null)
        {
            return BadRequest("File content cannot be empty");
        }

        try
        {
            // var result = await service.ProcessFileDetails(msg);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("download/{id}")]
    public async Task<IActionResult> Download([Required] string id)
    {
        if (!int.TryParse(id, out int intId))
        {
            return BadRequest("File ID must be a valid integer");
        }
        if (string.IsNullOrWhiteSpace(id))
        {
            return BadRequest("File ID cannot be empty");
        }

        try
        {
            var fileContent = await service.GetFileContentAsync(intId);
            
            if (fileContent == null)
            {
                return NotFound($"File with ID '{id}' not found");
            }
            
            return File(fileContent.Content, "application/octet-stream", fileContent.Filename);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}