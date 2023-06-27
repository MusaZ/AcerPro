using AcerPro.Models;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;

namespace AcerPro.InterfacesMethods;

public interface IStreamToLogger
{
  public string RawString { get; set; }
  public List<Logger> ConvertToLogger();  
}

public class StreamToLogger : IStreamToLogger
{
  public string RawString { get { return _rawString; } set { _rawString = value; } }
  private string _rawString;
  private string activeVal = string.Empty;

  public StreamToLogger(){}

  public StreamToLogger(string rawString)
  {
    _rawString = rawString;
  }

  public List<Logger> ConvertToLogger()
  {
    List<Logger> loggers = new List<Logger>();

    string assignRawString = _rawString;

    int order = 0, indx = 0;
    List<string> Ids = new List<string>();
    List<string> LogLevels = new List<string>();
    List<string> EventIds = new List<string>();
    List<string> Times = new List<string>();
    List<string> Messages = new List<string>();


    while (assignRawString.Contains("="))
    {
      int equalPos = assignRawString.IndexOf('=');
      int tirePos = order < 3 ? assignRawString.IndexOf('-') : int.MaxValue;
      if (order != 4)
        activeVal = assignRawString.Substring(0, tirePos < equalPos ? tirePos : equalPos);
      else
      {
        int quotaPos = assignRawString.IndexOf('"');
        assignRawString = assignRawString.Substring(++quotaPos);
        quotaPos = assignRawString.IndexOf('"');
        activeVal = assignRawString.Substring(0, quotaPos);
        assignRawString = assignRawString.Substring(++quotaPos);
      }
      
      switch (order)
      {
        case 0:
          {
            loggers.Add(new Logger());

            Ids.Add(activeVal);
            loggers[indx].userId = activeVal;

            break;
          }
        case 1:
          {
            var logLvl = activeVal.Replace("[", "").Replace("]", "");
            LogLevels.Add(logLvl);
            loggers[indx].logLevel = logLvl;

            break;
          }
        case 2:
          {
            EventIds.Add(activeVal);
            loggers[indx].eventId = new EventId(Convert.ToInt32(activeVal));

            break;
          }
        case 3:
          {
            Times.Add(activeVal);
            loggers[indx].time = TimeOnly.Parse(activeVal);

            break;
          }
        case 4:
          {
            Messages.Add(activeVal);
            loggers[indx].message = activeVal;
            indx++;
            order = 0;

            continue;
          }
      }

      assignRawString = assignRawString.Substring(order != 3 ? ++tirePos : ++equalPos);

      if (order != 4)
        order++;      
    }

    //var zipp = Ids.Zip(LogLevels, EventIds);
    //var zipp1 = zipp.Zip(Times, Messages);
    //zipp1.ToList().ForEach((tupl) => { tupl.Third = tupl.First.First + tupl.Second; }); [tupl = (string s, string s1, string s2) first, string second, string third]

    return loggers;
  }
}
