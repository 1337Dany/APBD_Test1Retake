using APBD_Test1Retake.DTOs;

namespace APBD_Test1Retake.Services.Interfaces;

public interface IMovieService
{
    Task<List<MovieDetailsDTO>> GetAllMoviesAsync(DateTime? releaseDateFrom, DateTime? releaseDateTo);
    Task<int> AddNewActorAsync(NewActorMovieDTO dto);
}