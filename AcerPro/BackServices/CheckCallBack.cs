using System.Net;
using System.Net.Mail;
using AcerPro.InterfacesMethods;
using AcerPro.Models;
using StackExchange.Redis;

namespace AcerPro.BackServices;

public class CheckCallBack : IHostedService
{
  public int Id { get; set; }
  public int UserId { get; set; }
  public string? Url { get; set; }
  public string? AppName { get; set; }
  public IEnumerable<string> MailAddress { get; set; }
  public int? Interval { get; set; }  

  private readonly ILogger<CheckCallBack> _logger = new Logger<CheckCallBack>(new LoggerFactory());

  private Timer _timer;
  //---------------------------------------------------------------------------------------------------------------------
  private void CheckAppHealth(object? _object)
  {
    int RespCode = 200;
    var redisDB = GlobalVariables.GlobalVariables.connectionMultiplexer.GetDatabase();
    try
    {
      RespCode = CheckURLCode(Url).Result;

      if (RespCode >= 200 && RespCode < 300)
      {        
        redisDB.HashSetAsync("Apps", $"{AppName}", new RedisValue("1"));
        return;
      }
    }
    catch (Exception e)
    {
      RespCode = 500;
      redisDB.HashSetAsync("Apps", $"{AppName}", new RedisValue("2"));
      _logger.LogError($"Error In \"CheckAppHealth\". Details : \n {e.Message}\n{e.Source}");
      //throw e;
    }
    finally
    {
      if (RespCode >= 300)
      {
        var config = GlobalVariables.GlobalVariables.ConfigurationManager;
        NetworkCredential networkCredential = new NetworkCredential(config!["MailUser"], config["MailPass"]);
        MailHostData mailHostData = new MailHostData()
        {
                Host = "smtp-mail.outlook.com",
                Port = 587,
                EnableSSL = true,
                SenderandFrom = new MailAddress(config["MailUser"]!)
        };
        INotification notification =
                new MailNotification(networkCredential, mailHostData, AppName, MailAddress);
        notification.DoNotificationAsync();
      }
    }
  }
  //---------------------------------------------------------------------------------------------------------------------
  private async Task<int> CheckURLCode(string Url)
  {
    HttpClient httpClient = new HttpClient();
    httpClient.BaseAddress = new Uri(Url);
    var response = await httpClient.GetAsync(httpClient.BaseAddress);

    return (int)response.StatusCode;
  }
  //---------------------------------------------------------------------------------------------------------------------
  public Task StartAsync(CancellationToken cancellationToken)
  {
    _timer = new Timer(CheckAppHealth, null, TimeSpan.Zero, TimeSpan.FromSeconds(Interval!.Value));

    return Task.CompletedTask;
  }
  //---------------------------------------------------------------------------------------------------------------------
  public Task StopAsync(CancellationToken cancellationToken)
  {
    //_timer.Change(Timeout.Infinite, 0);
    _timer.Change(-1, 0); //TimeOut.Infinite is Equal to -1.

    return Task.CompletedTask;
  }
}