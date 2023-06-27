using StackExchange.Redis;

namespace AcerPro.CustomLogger;

public class LoggerProvider : ILoggerProvider
{
  private readonly IConnectionMultiplexer _connectionMultiplexer;

  public LoggerProvider(IConnectionMultiplexer connectionMultiplexer)
  {
    _connectionMultiplexer = connectionMultiplexer;
  }

  public ILogger CreateLogger(string categoryName)
  {
    return new CustomLogger(_connectionMultiplexer);
  }

  public void Dispose()
  {
    
  }
}
