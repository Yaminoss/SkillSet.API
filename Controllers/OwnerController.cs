using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using SkillSet.API.Services;
using Skillest.API.Application;
using Skillest.API.Extenstions;
using Newtonsoft.Json;
using Skillest.API.DTO;

namespace SkillSet.API.Controllers;


[ApiController]
[Route("[controller]")]
public class OwnerController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;
    private readonly IOwnerApplication _ownerApplication;

    public OwnerController(IWebHostEnvironment _environment, IOwnerApplication _ownerApplication)
    {
        this._environment = _environment;
        this._ownerApplication = _ownerApplication;
    }

    [HttpPost("GetOwnersFile/{provinces?}"), DisableRequestSizeLimit]
    public async Task<IActionResult> Upload(string provinces = "")
    {
        try
        {
            var file = Request.Form.Files[0];
            var folderName = Path.Combine("Resources", "Owners");
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            if (file.Length > 0)
            {
                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName?.Trim('"');
                var fullPath = Path.Combine(pathToSave, fileName + "_" + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss"));
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                var selectedOwners = OwnerXmlParser.Parse(fullPath, provinces.Split(',').ToList());
                if (await _ownerApplication.AddOwners(selectedOwners))
                    return Ok();

                return BadRequest("Owners failed to be add or updated");
            }
            else
                return BadRequest("File filed to be parsed");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex}");
        }
    }

    [HttpPost("GetAllOwners")]
    public async Task<IActionResult> GetAllOwners([FromBody] OwnerParameters ownerParameters)
    {
        var owners = _ownerApplication.GetAllOwners(ownerParameters);
        if (owners.owners.Count > 0)
        {
            return Ok(owners);
        }
        return Ok("No owners found");
    }
}