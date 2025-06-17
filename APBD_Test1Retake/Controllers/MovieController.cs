using APBD_Test1Retake.DTOs;
using APBD_Test1Retake.Exceptions;
using APBD_Test1Retake.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace APBD_Test1Retake.Controllers;

[ApiController]
public class Controller(IService service) : ControllerBase
{
    [HttpGet("api/[controller]/{id}")]
    public async Task<ActionResult<List<ModelDTO>>> Get(int id)
    {
        try
        {
            var tmp = await service.GetAllAsync(id);
            return Ok(tmp);
        }
        catch (ServerResponseError e)
        {
            return BadRequest(e.Message); //400, NotFound 404,   
        }
    }

    [HttpPost("api/[controller]")]
    public async Task<ActionResult<int>> AddActor(NewActorDTO dto)
    {
        try
        {
            var tmp = await service.AddAsync(dto);
            return Ok(tmp);
        }
        catch (ServerResponseError e)
        {
            return BadRequest(e.Message); //400, NotFound 404,   
        }
    }
    
    
}