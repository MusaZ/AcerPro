namespace AcerPro.Models;

public class Logger
{
  public string userId { get; set; }
  public EventId eventId { get; set; }
  public string logLevel{ get; set; }
  public TimeOnly time{ get; set; }
  public string message { get; set; }
}

