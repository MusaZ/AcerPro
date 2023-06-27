using AcerPro.BackServices;
using AcerPro.Models;
using StackExchange.Redis;

namespace AcerPro.GlobalVariables;

public static class GlobalVariables
{
  public static ConfigurationManager? ConfigurationManager;
  public static List<CheckCallBack>? BackCheckServices = new();
  public static IConnectionMultiplexer connectionMultiplexer { get; set; }
  public static FileStream? FileStream { get; set; } = null;
}