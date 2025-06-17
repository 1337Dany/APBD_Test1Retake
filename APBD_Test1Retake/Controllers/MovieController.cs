using APBD_Test1Retake.DTOs;
using APBD_Test1Retake.Exceptions;
using APBD_Test1Retake.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace APBD_Test1Retake.Controllers;

[ApiController]
public class MovieController(IMovieService movieService) : ControllerBase
{
    [HttpGet("api/[controller]")]
    public async Task<ActionResult<List<MovieActorDTO>>> GetAllMovies(
        [FromQuery] DateTime? releaseDateFrom, 
        [FromQuery] DateTime? releaseDateTo
        )
    {
        try
        {
            var movies = await movieService.GetAllMoviesAsync(releaseDateFrom, releaseDateTo);
            return Ok(movies);
        }
        catch (ServerResponseError e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("api/[controller]")]
    public async Task<ActionResult<int>> AddActor(NewActorMovieDTO movieDto)
    {
        try
        {
            var id = await movieService.AddNewActorAsync(movieDto);
            return Ok(id);
        }
        catch (MovieNotFoundError e)
        {
            return NotFound(e.Message); 
        }
        catch (ActorNotFoundError e)
        {
            return NotFound(e.Message);
        }
        catch (ServerResponseError e)
        {
            return BadRequest(e.Message);
        }
    }
}