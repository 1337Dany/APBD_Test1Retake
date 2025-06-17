using System.ComponentModel.DataAnnotations;

namespace APBD_Test1Retake.DTOs;

public class NewActorMovieDTO
{
    [Required]
    public int IdMovie { get; set; }

    [Required]
    public int IdActor { get; set; }

    [Required]
    [StringLength(50)]
    public string CharacterName { get; set; }
}