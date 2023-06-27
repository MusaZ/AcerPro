namespace AcerPro.Models;

public class ErrorModel
{
  public int Id { get; set; }
  public string Mesaj { get; set; }
  public string Kaynak { get; set; }
  public string StackTrc { get; set; }
}