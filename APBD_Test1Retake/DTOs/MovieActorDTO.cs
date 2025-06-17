namespace APBD_Test1Retake.DTOs;

public class MovieActorDTO
{
    public int IdMovie { get; set; }
    public string MovieName { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string? AgeRatingName { get; set; }

    public int? IdActor { get; set; }
    public string? ActorName { get; set; }
    public string? ActorSurname { get; set; }
    public string? CharacterName { get; set; }
}