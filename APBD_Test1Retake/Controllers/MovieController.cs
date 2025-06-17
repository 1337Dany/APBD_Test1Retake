using APBD_Test1Retake.DTOs;
using APBD_Test1Retake.Exceptions;
using APBD_Test1Retake.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace APBD_Test1Retake.Controllers;
[Route("api/movies")]
[ApiController]
public class MovieController(IMovieService movieService) : ControllerBase
{
    [HttpGet]
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
            return Problem(e.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<int>> AddActor(NewActorMovieDTO movieDto)
    {
        try
        {
            var id = await movieService.AddNewActorAsync(movieDto);
            return Created(id.ToString(), id);
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
            return Problem(e.Message);
        }
    }
}