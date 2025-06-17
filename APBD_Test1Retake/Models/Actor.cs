using System.ComponentModel.DataAnnotations;

namespace APBD_Test1Retake.Models;

public class Actor
{
    public int IdActor { get; set; }

    [Required]
    [StringLength(30)]
    public string Name { get; set; }

    [Required]
    [StringLength(30)]
    public string Surname { get; set; }
}