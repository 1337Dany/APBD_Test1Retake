using System.ComponentModel.DataAnnotations;

namespace APBD_Test1Retake.Models;

public class AgeRating
{
    public int IdRating { get; set; }
    [Required]
    [StringLength(30)]
    public string Name { get; set; }
}
