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
}