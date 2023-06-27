using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AcerPro.Models;

public class LoginData
{
  [Required(ErrorMessage = "Kullanıcı Adı Girilmesi Zorunlu")]
  public string UserName { get; set; }
  
  [Required(ErrorMessage = "Şifre Girilmesi Zorunlu")]
  [DataType(DataType.Password)]
  public string Password { get; set; }

  public bool? IsSuccess { get; set; } = true;
}