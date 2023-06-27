using System.Net.Mail;

namespace AcerPro.Models;

public class MailHostData
{
  public string Host { get; set; }
  public int Port { get; set; }
  public bool EnableSSL { get; set; }
  public MailAddress SenderandFrom { get; set; }
}