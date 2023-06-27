using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AcerPro.Models;

public class Users
{
  public int Id { get; set; }
  
  [Required(ErrorMessage = "Kullanıcı Adı Alanı Boş Bırakılamaz")]
  public string UserName { get; set; }
  
  [Required(ErrorMessage = "Kullanıcı Parola Alanı Boş Bırakılamaz")]
  [DataType(DataType.Password)]
  public string Password { get; set; }
  
  [Required(ErrorMessage = "Kullanıcı EMail Alanı Boş Bırakılamaz")]
  [EmailAddress]
  public required string EMail { get; set; }
  //public string EMail { get; set; }

  List<TargetApplications> TargetApplications { get; set; }
}