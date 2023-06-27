using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices.JavaScript;

namespace AcerPro.Models;

public class TargetApplications
{
  [Key]
  public int Id { get; set; }
  
  [ForeignKey(nameof(Users.Id))]
  public int UserId { get; set; }

  [Required(ErrorMessage = "Uygulama İsmi Girilmesi Gerekli.")]
  [MaxLength(25)]
  public string AppName { get; set; }
  
  [Required(ErrorMessage = "Uygulama URL'si Zorunlu bir Alandır.")]
  public string? AppUrl { get; set; }

  [Required(ErrorMessage = "Tarama Zamanı Zorunlu bir Alandır.")]
  [DataType(DataType.Time)]
  public int? Interval { get; set; }

}