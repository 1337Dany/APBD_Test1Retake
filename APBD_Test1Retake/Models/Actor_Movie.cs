using System.ComponentModel.DataAnnotations;

namespace APBD_Test1Retake.Models;

public class Actor_Movie
{
    public int IdActorMovie { get; set; }

    [Required]
    public int IdMovie { get; set; }

    [Required]
    public int IdActor { get; set; }

    [Required]
    [StringLength(50)]
    public string CharacterName { get; set; }

    public Movie Movie { get; set; }
    public Actor Actor { get; set; }
}
