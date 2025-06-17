using APBD_Test1Retake.DTOs;
using APBD_Test1Retake.Exceptions;
using APBD_Test1Retake.Models;
using APBD_Test1Retake.Services.Interfaces;
using Microsoft.Data.SqlClient;

namespace APBD_Test1Retake.Services;

public class MovieService : IMovieService
{
    private readonly string _connectionString;

    public MovieService(IConfiguration cfg)
    {
        _connectionString = cfg.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException(
            nameof(cfg),
            "DefaultConnection connection string configuration is missing");
    }

    public async Task<List<MovieDetailsDTO>> GetAllMoviesAsync(DateTime? releaseDateFrom, DateTime? releaseDateTo)
    {
        var result = new List<MovieDetailsDTO>();

        await using (SqlConnection connection = new(_connectionString))
        {
            await connection.OpenAsync();

            string sql = @"
            SELECT 
                m.IdMovie, m.Name AS MovieName, m.ReleaseDate,
                ar.Name AS AgeRatingName,
                a.IdActor, a.Name AS ActorName, a.Surname, am.CharacterName
            FROM Movie m
            LEFT JOIN AgeRating ar ON m.IdAgeRating = ar.IdRating
            LEFT JOIN Actor_Movie am ON m.IdMovie = am.IdMovie
            LEFT JOIN Actor a ON am.IdActor = a.IdActor
            WHERE (@ReleaseDateFrom IS NULL OR m.ReleaseDate >= @ReleaseDateFrom)
              AND (@ReleaseDateTo IS NULL OR m.ReleaseDate <= @ReleaseDateTo)
            ORDER BY m.ReleaseDate DESC";

            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@ReleaseDateFrom", (object?)releaseDateFrom ?? DBNull.Value);
                command.Parameters.AddWithValue("@ReleaseDateTo", (object?)releaseDateTo ?? DBNull.Value);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    var movieDict = new Dictionary<int, MovieDetailsDTO>();

                    while (await reader.ReadAsync())
                    {
                        int movieId = reader.GetInt32(reader.GetOrdinal("IdMovie"));

                        if (!movieDict.ContainsKey(movieId))
                        {
                            movieDict[movieId] = new MovieDetailsDTO
                            {
                                IdMovie = movieId,
                                Name = reader.GetString(reader.GetOrdinal("MovieName")),
                                ReleaseDate = reader.GetDateTime(reader.GetOrdinal("ReleaseDate")),
                                AgeRatingName = reader.IsDBNull(reader.GetOrdinal("AgeRatingName"))
                                    ? null
                                    : reader.GetString(reader.GetOrdinal("AgeRatingName")),
                                Actors = new List<MovieActorDTO>()
                            };
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("IdActor")))
                        {
                            var actor = new MovieActorDTO
                            {
                                IdActor = reader.GetInt32(reader.GetOrdinal("IdActor")),
                                Name = reader.GetString(reader.GetOrdinal("ActorName")),
                                Surname = reader.GetString(reader.GetOrdinal("Surname")),
                                CharacterName = reader.GetString(reader.GetOrdinal("CharacterName"))
                            };

                            movieDict[movieId].Actors.Add(actor);
                        }
                    }

                    result.AddRange(movieDict.Values);
                }
            }
        }

        return result;
    }

    public async Task<int> AddNewActorAsync(NewActorMovieDTO dto)
    {
        await using (SqlConnection connection = new(_connectionString))
        {
            await connection.OpenAsync();
            await using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    using (SqlCommand checkMovieCmd =
                           new SqlCommand("SELECT COUNT(1) FROM Movie WHERE IdMovie = @IdMovie", connection,
                               transaction))
                    {
                        checkMovieCmd.Parameters.AddWithValue("@IdMovie", dto.IdMovie);
                        int movieExists = (int)await checkMovieCmd.ExecuteScalarAsync();

                        if (movieExists == 0)
                            throw new MovieNotFoundError("Movie not found");
                    }

                    using (SqlCommand checkActorCmd =
                           new SqlCommand("SELECT COUNT(1) FROM Actor WHERE IdActor = @IdActor", connection,
                               transaction))
                    {
                        checkActorCmd.Parameters.AddWithValue("@IdActor", dto.IdActor);
                        int actorExists = (int)await checkActorCmd.ExecuteScalarAsync();

                        if (actorExists == 0)
                            throw new ActorNotFoundError("Actor not found");
                    }

                    const string queryString = @"
                INSERT INTO Actor_Movie (IdMovie, IdActor, CharacterName)
                VALUES (@IdMovie, @IdActor, @CharacterName);"
                                               + "SELECT CAST(SCOPE_IDENTITY() AS INT);";

                    using (SqlCommand command = new SqlCommand(queryString, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@IdMovie", dto.IdMovie);
                        command.Parameters.AddWithValue("@IdActor", dto.IdActor);
                        command.Parameters.AddWithValue("@CharacterName", dto.CharacterName);

                        var id = await command.ExecuteScalarAsync();

                        await transaction.CommitAsync();

                        return id != null
                            ? Convert.ToInt32(id)
                            : throw new ServerResponseError("Id of new entity was not found");
                    }
                }
                catch (SqlException ex)
                {
                    await transaction.RollbackAsync();
                    throw new ServerResponseError("Server returned an error: " + ex.Message);
                }
            }
        }
    }
}