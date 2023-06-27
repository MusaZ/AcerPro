using AcerPro.BackServices;
using AcerPro.Models;

namespace AcerPro.InterfacesMethods.CallBackServices;

public class AddNewCallBack
{
  private readonly IServiceProvider _serviceProvider;
  private readonly TargetApplications _targetApplications;
  private readonly string? _eMail;
  private readonly int _userId;

  //----------------------------------------------------------------------------------------------------------------------
  public AddNewCallBack(string? eMail, TargetApplications targetApplications, IServiceProvider serviceProvider, int userId)
  {
    _eMail = eMail;
    _targetApplications = targetApplications;
    _serviceProvider = serviceProvider;    
    _userId = userId;

    AddChecker();
  }
  //----------------------------------------------------------------------------------------------------------------------
  private void AddChecker()
  {
    try
    {
      var checkCallBack = _serviceProvider.GetService<CheckCallBack>();
      checkCallBack!.Id = _targetApplications.Id;
      checkCallBack.Interval = _targetApplications.Interval;
      checkCallBack.AppName = _targetApplications.AppName;
      checkCallBack.Url = _targetApplications.AppUrl;
      checkCallBack.UserId = _userId;
      checkCallBack.MailAddress = new[] { _eMail };
      checkCallBack.StartAsync(new CancellationToken());

      var backCheckService = _serviceProvider.GetService<IBackCheckServices>();
      backCheckService!.CheckCallBacks.Add(checkCallBack);
      //GlobalVariables.GlobalVariables.BackCheckServices!.Add(checkCallBack);
    }
    catch (Exception e)
    {
      throw;
    }
  }
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////