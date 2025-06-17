using APBD_Test1Retake.DTOs;

namespace APBD_Test1Retake.Services.Interfaces;

public interface IMovieService
{
    Task<List<MovieActorDTO>> GetAllMoviesAsync(DateTime? releaseDateFrom, DateTime? releaseDateTo);
    Task<int> AddNewActorAsync(NewActorMovieDTO dto);
}