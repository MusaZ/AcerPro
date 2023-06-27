using StackExchange.Redis;
using System.Globalization;
using System.Text;

namespace AcerPro.CustomLogger;

public class CustomLogger : ILogger
{
  private readonly IConnectionMultiplexer _connectionMultiplexer;

  public CustomLogger(IConnectionMultiplexer connectionMultiplexer)
  {
    _connectionMultiplexer = connectionMultiplexer;
  }

  public IDisposable? BeginScope<TState>(TState state) where TState : notnull
  {
    return null;
  }

  public bool IsEnabled(LogLevel logLevel)
  {
    return true;
  }

  public async void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
  {
    string logTime = DateTime.Now.ToString("HH:mm:ss");
    var redisDb = _connectionMultiplexer.GetDatabase();
    string usrId = redisDb.StringGetAsync("UserID").Result.ToString();

    await WriteToFileAsync(usrId, logLevel, eventId, logTime, formatter(state, exception));
  }

  private async Task WriteToFileAsync(string userId, LogLevel logLevel, EventId id, string time, string? Message)
  {
    var appPath = Directory.GetCurrentDirectory() + "/Logs/";
    var logName = DateTime.Now.ToString("dd.MM.yyyy") + ".log";
    var filePath = string.Concat(appPath, logName);

    if (!File.Exists(filePath))
    {
      var FStr = File.Create(filePath);
      FStr.Close();

      GlobalVariables.GlobalVariables.FileStream = new FileStream(filePath, FileMode.Append);
    }

    FileStream? fileStream = GlobalVariables.GlobalVariables.FileStream;

    if (fileStream is null)
    {
      fileStream = new FileStream(filePath, FileMode.Append);
      GlobalVariables.GlobalVariables.FileStream = fileStream;
      
      if (fileStream is { CanWrite : true })
      {
        string logData = $"{userId}-[{logLevel}]-{id.Id.ToString()}-{time} = \"{ Message}\"\n";
        var spanData = Encoding.UTF8.GetBytes(logData);
        ReadOnlyMemory<byte> readOnlyMemory = new ReadOnlyMemory<byte>(spanData);

        await fileStream.WriteAsync(readOnlyMemory);

        fileStream.Flush();
        fileStream.Close();
      }
      //await File.AppendAllTextAsync(filePath, $"{userId}-[{logLevel}]-{id.Id.ToString()}-{time} = \"{Message}\"\n");
    }
    else if(fileStream.CanWrite)
      await File.AppendAllTextAsync(filePath, $"{userId}-[{logLevel}]-{id.Id.ToString()}-{time} = \"{Message}\"\n");
  }
}
