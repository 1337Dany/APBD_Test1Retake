using APBD_Test1Retake.DTOs;
using APBD_Test1Retake.Exceptions;
using APBD_Test1Retake.Models;
using APBD_Test1Retake.Services.Interfaces;
using Microsoft.Data.SqlClient;

namespace APBD_Test1Retake.Services;

public class MovieService : IService
{
    const string connectionString =
        "Data Source=db-mssql;Initial Catalog=2019SBD;Integrated Security=True;TrustServerCertificate=True";

    public async Task<List<MovieActorDTO>> GetAllMoviesAsync(DateTime? releaseDateFrom, DateTime? releaseDateTo)
    {
        const string queryString = @"
            SELECT 
                m.IdMovie, m.Name AS MovieName, m.ReleaseDate,
                ar.Name AS AgeRatingName,
                a.IdActor, a.Name AS ActorName, a.Surname, am.CharacterName
            FROM Movie m
            LEFT JOIN AgeRating ar ON m.IdAgeRating = ar.IdRating
            LEFT JOIN ActorMovie am ON m.IdMovie = am.IdMovie
            LEFT JOIN Actor a ON am.IdActor = a.IdActor
            WHERE (@ReleaseDateFrom IS NULL OR m.ReleaseDate >= @ReleaseDateFrom)
              AND (@ReleaseDateTo IS NULL OR m.ReleaseDate <= @ReleaseDateTo)
            ORDER BY m.ReleaseDate DESC";

        await using (SqlConnection connection = new(connectionString))
        {
            SqlCommand command = new(queryString, connection);
            command.Parameters.AddWithValue("@ReleaseDateFrom", (object?)releaseDateFrom ?? DBNull.Value);
            command.Parameters.AddWithValue("@ReleaseDateTo", (object?)releaseDateTo ?? DBNull.Value);

            try
            {
                
                var result = new List<MovieActorDTO>();
                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var dto = new MovieActorDTO
                        {
                            IdMovie = reader.GetInt32(reader.GetOrdinal("IdMovie")),
                            MovieName = reader.GetString(reader.GetOrdinal("MovieName")),
                            ReleaseDate = reader.GetDateTime(reader.GetOrdinal("ReleaseDate")),
                            AgeRatingName = reader.IsDBNull(reader.GetOrdinal("AgeRatingName")) 
                                ? null 
                                : reader.GetString(reader.GetOrdinal("AgeRatingName")),

                            IdActor = reader.IsDBNull(reader.GetOrdinal("IdActor")) 
                                ? null 
                                : reader.GetInt32(reader.GetOrdinal("IdActor")),
                            ActorName = reader.IsDBNull(reader.GetOrdinal("ActorName")) 
                                ? null 
                                : reader.GetString(reader.GetOrdinal("ActorName")),
                            ActorSurname = reader.IsDBNull(reader.GetOrdinal("Surname")) 
                                ? null 
                                : reader.GetString(reader.GetOrdinal("Surname")),
                            CharacterName = reader.IsDBNull(reader.GetOrdinal("CharacterName")) 
                                ? null 
                                : reader.GetString(reader.GetOrdinal("CharacterName")),
                        };

                        result.Add(dto);
                    }
                }

                return result;
            }
            catch (SqlException ex)
            {
                throw new ServerResponseError("Server returned an error: " + ex.Message);
            }
        }
    }

    public async Task<int> AddNewActorAsync(NewActorMovieDTO dto)
    {
        await using (SqlConnection connection = new(connectionString))
        {
            await connection.OpenAsync();
            
            using (SqlCommand checkMovieCmd =
                   new SqlCommand("SELECT COUNT(1) FROM Movie WHERE IdMovie = @IdMovie", connection))
            {
                checkMovieCmd.Parameters.AddWithValue("@IdMovie", dto.IdMovie);
                int movieExists = (int)await checkMovieCmd.ExecuteScalarAsync();

                if (movieExists == 0)
                    throw new MovieNotFoundError("Movie not found");
            }

            using (SqlCommand checkActorCmd =
                   new SqlCommand("SELECT COUNT(1) FROM Actor WHERE IdActor = @IdActor", connection))
            {
                checkActorCmd.Parameters.AddWithValue("@IdActor", dto.IdActor);
                int actorExists = (int)await checkActorCmd.ExecuteScalarAsync();

                if (actorExists == 0)
                    throw new ActorNotFoundError("Actor not found");
            }
            await using (var transaction = connection.BeginTransaction())
            {
                const string queryString = @"
                INSERT INTO ActorMovie (IdMovie, IdActor, CharacterName)
                VALUES (@IdMovie, @IdActor, @CharacterName);"
                                           + "SELECT CAST(SCOPE_IDENTITY() AS INT);";

                SqlCommand command = new(queryString, connection);
                command.Parameters.AddWithValue("@IdMovie", dto.IdMovie);
                command.Parameters.AddWithValue("@IdActor", dto.IdActor);
                command.Parameters.AddWithValue("@CharacterName", dto.CharacterName);

                try
                {
                    var id = await command.ExecuteScalarAsync();

                    await transaction.CommitAsync();

                    return id != null
                        ? Convert.ToInt32(id)
                        : throw new ServerResponseError("Id of new entity was not found");
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