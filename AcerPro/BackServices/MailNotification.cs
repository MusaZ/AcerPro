using System.Net;
using System.Net.Mail;
using AcerPro.InterfacesMethods;
using AcerPro.Models;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace AcerPro.BackServices;

public class MailNotification : INotification
{
  private readonly NetworkCredential _networkCredential;
  private readonly MailHostData _mailHostData;
  private readonly string _appName;

  private readonly ILogger<MailNotification> _logger = new Logger<MailNotification>(new LoggerFactory());
  /// <summary>
  /// 
  /// </summary>
  /// <param name="networkCredential">Gönderici Mail Hesabı İçin Zorunlu Giriş Bilgileri.</param>
  /// <param name="mailHostData">Gönderici Mail Hesab Host Bilgileri.</param>
  /// <param name="appName">Offline Olan Hedef Uygulama İsmi.</param>
  /// <param name="destinations">Bilgi Verilecek Kişiler.</param>
   
  public MailNotification(NetworkCredential networkCredential, 
          MailHostData mailHostData, 
          string appName, 
          IEnumerable<string> destinations)
  {
    _networkCredential = networkCredential;
    _mailHostData = mailHostData;
    _appName = appName;
    Destinations = destinations;
  }
  //---------------------------------------------------------------------------------------------------------------------
  public IEnumerable<string> Destinations { get; set; }
  //---------------------------------------------------------------------------------------------------------------------
  public async Task DoNotificationAsync()
  {
    try
    {
      using SmtpClient smtpClient = new SmtpClient();
      using MailMessage mailMessage = new MailMessage();

      smtpClient.Credentials = _networkCredential;
      smtpClient.Host = _mailHostData.Host;
      smtpClient.Port = _mailHostData.Port;

      mailMessage.From = mailMessage.Sender = _mailHostData.SenderandFrom;
      mailMessage.Subject = "Target App Has Been Down";
      mailMessage.Body = $"{_appName} İsimli Hedef Uygulaması Offline Olmuştur.";
      for (int a = 0; a < Destinations.Count(); a++)
      {
        mailMessage.To.Add(new MailAddress(Destinations.ElementAt(a)));
      }

      smtpClient.EnableSsl = true;
      await smtpClient.SendMailAsync(mailMessage);

      smtpClient.SendCompleted += (sender, args) =>
      {
        if(args.Cancelled || args.Error is not null)
          _logger.LogError($"Error Occured During Mail Sending.");
      };
    }
    catch (Exception e)
    {
      //throw e;
    }
  }
}
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////