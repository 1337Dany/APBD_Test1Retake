using System.ComponentModel.DataAnnotations;

namespace APBD_Test1Retake.Models;

public class Movie
{
    public int IdMovie { get; set; }

    [Required]
    public int IdAgeRating { get; set; }

    [Required]
    [StringLength(30)]
    public string Name { get; set; }

    [Required]
    public DateTime ReleaseDate { get; set; }

    public AgeRating AgeRating { get; set; }
}