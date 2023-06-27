using AcerPro.BackServices;
using AcerPro.EFCore;
using AcerPro.Models;
using AcerPro.Repostory;
using System.Runtime.CompilerServices;

namespace AcerPro.InterfacesMethods.CallBackServices;

public class LoadCheckCallback
{
    private readonly IServiceProvider _serviceProvider;
    private readonly EfDbClass _efDbClass;
    private readonly string? _mailAddr;
    private readonly int _userId;

    public LoadCheckCallback(IServiceProvider serviceProvider, EfDbClass efDbClass, string? mailAddr, int userId, IBackCheckServices backCheckServices)
    {
        _serviceProvider = serviceProvider;
        _efDbClass = efDbClass;
        _mailAddr = mailAddr;
        _userId = userId;

        Load(backCheckServices);
    }

    private void Load(IBackCheckServices backCheckServices)
    {
        try
        {
            IRepository<TargetApplications> targetApps = new ApplicaitonsRepository(_efDbClass, _serviceProvider);
            var userApps = targetApps.Repos.Where(x => x.UserId == _userId);

            Parallel.ForEach(userApps, apps =>
            {
                CheckCallBack checkCallBack = _serviceProvider.GetService<CheckCallBack>()!;
                checkCallBack.Id = apps.Id;
                checkCallBack.UserId = _userId;
                checkCallBack.Interval = apps.Interval;
                checkCallBack.AppName = apps.AppName;
                checkCallBack.Url = apps.AppUrl;
                checkCallBack.MailAddress = new[] { _mailAddr };
                checkCallBack.StartAsync(new CancellationToken());

                backCheckServices!.CheckCallBacks.Add(checkCallBack);
            });
        }
        catch (Exception e)
        {
            throw;
        }
    }
}

#region DISABLE
/*
for (int a = 0; a < targetApps.Repos.Count(); a++)
{
  CheckCallBack checkCallBack = _serviceProvider.GetService<CheckCallBack>()!;
  checkCallBack.Interval = targetApps.Repos.ElementAt(a).Interval;
  checkCallBack.AppName = targetApps.Repos.ElementAt(a).AppName;
  checkCallBack.Url = targetApps.Repos.ElementAt(a).AppUrl;
  checkCallBack.MailAddress = _mailAddr;

  GlobalVariables.GlobalVariables.BackCheckServices!.Add(checkCallBack);
}
*/
#endregion