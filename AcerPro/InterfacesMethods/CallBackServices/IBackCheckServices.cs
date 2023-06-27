using AcerPro.BackServices;
using Microsoft.VisualBasic;

namespace AcerPro.InterfacesMethods.CallBackServices;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
public interface IBackCheckServices
{
  public List<CheckCallBack> CheckCallBacks { get; set; }

  public void RemoveBackCheckService(int userId, IEnumerable<int>? AppId = null);
}
//----------------------------------------------------------------------------------------------------------------------
public class BackCheckServices : IBackCheckServices
{
  public List<CheckCallBack> CheckCallBacks { get; set; }
  //--------------------------------------------------------------------------------------------------------------------
  public BackCheckServices()
  {
    CheckCallBacks = new List<CheckCallBack>();
  }
  //--------------------------------------------------------------------------------------------------------------------
  public void RemoveBackCheckService(int userId, IEnumerable<int>? AppId = null)
  {
    switch(AppId)
    {
      case null:
      {
        var backServices = CheckCallBacks.Where(_ => _.UserId == userId);
        for (int a = 0; a < backServices.Count(); a++)
        {
          backServices.ElementAt(a).StopAsync(new CancellationToken());
        }
        
        this.CheckCallBacks.RemoveAll(_ => _.UserId == userId);

        break;
      }
      default:
      {
        var backServices = CheckCallBacks.Where(_ => _.UserId == userId).ToList();
        for (int a = 0; a < backServices.Count(); a++)
        {
          for (int b = 0; b < AppId.Count(); b++)
          {
            var backService = backServices.FirstOrDefault(_ => _.Id == AppId.ElementAt(b));
            if(backService is not null)
              backService.StopAsync(new CancellationToken());
          }
        }

        for (int i = 0; i < AppId.Count(); i++)
        {
          this.CheckCallBacks.RemoveAll(_ => _.UserId == userId && _.Id == AppId.ElementAt(i));
        }

        break;
      }
    }    
  }
}
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////