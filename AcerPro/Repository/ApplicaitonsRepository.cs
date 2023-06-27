using AcerPro.DataLayer;
using AcerPro.EFCore;
using AcerPro.InterfacesMethods.CallBackServices;
using AcerPro.Models;
using AcerPro.Repository;
using Microsoft.EntityFrameworkCore;

namespace AcerPro.Repostory;

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
public class ApplicaitonsRepository : IAppRepository<TargetApplications, UpdateData, DeleteData>
{
  private readonly EfDbClass _efDbClass;  
  private readonly IServiceProvider _serviceProvider;
  //----------------------------------------------------------------------------------------------------------------------------
  public ApplicaitonsRepository(EfDbClass efDbClass, IServiceProvider serviceProvider)
  {
    _efDbClass = efDbClass;
    _serviceProvider = serviceProvider;
  }
  //----------------------------------------------------------------------------------------------------------------------------
  public IEnumerable<TargetApplications> Repos => _efDbClass.DbApplications;
  //----------------------------------------------------------------------------------------------------------------------------
  public async Task AddApplicationAsync(TargetApplications targetApplications, int userId, string userMail)
  {    
    await _efDbClass.DbApplications.AddAsync(new()
    {
      AppName = targetApplications.AppName,
      AppUrl = targetApplications.AppUrl,
      Interval = targetApplications.Interval,
      UserId = userId
    });

    await _efDbClass.SaveChangesAsync();

    var willAddService = _efDbClass.DbApplications.First(_ => _.AppName == targetApplications.AppName);
    AddNewCallBack(userId, userMail, willAddService);
  }
  //----------------------------------------------------------------------------------------------------------------------------
  private void AddNewCallBack(int userId, string userMail, TargetApplications targetApplications)
  {
    var newCallBack =
            new AddNewCallBack(userMail, targetApplications, _serviceProvider, userId);
  }
  //----------------------------------------------------------------------------------------------------------------------------
  public async Task UpdateDataAsync(UpdateData updateData)
  {
    await UpdateDataAsync(new TargetApplications()
    {
      AppName = updateData.AppName,
      AppUrl = updateData.AppUrl,
      Id = updateData.Id,
      Interval = updateData.Interval
    });
  }
  //----------------------------------------------------------------------------------------------------------------------------
  public async Task DeleteDataAsync(int userId, DeleteData deleteData)
  {
    var willDelBackService = _serviceProvider.GetService<IBackCheckServices>();
    willDelBackService!.RemoveBackCheckService(userId, new[] { deleteData.Id });    

    var willRemoveApp = _efDbClass.DbApplications.First(_ => _.Id == deleteData.Id);

    var redisRemover = _serviceProvider.GetService<IRedisDBProcess>();
    await redisRemover!.RemoveHashDataAsync("Apps", new[] { willRemoveApp.AppName });

    await DeleteDataAsync(willRemoveApp);
  }
  //----------------------------------------------------------------------------------------------------------------------------
  public async Task DeleteDataAsync(TargetApplications targetApplications)
  {
    await _efDbClass.DbApplications.Where(_=>_.Id == targetApplications.Id).ExecuteDeleteAsync();
    //var backServiceCheck = _serviceProvider.GetService<IBackCheckServices>();
    //backServiceCheck!.CheckCallBacks.RemoveAll(_ => _.Id == targetApplications.Id);
  }
  //----------------------------------------------------------------------------------------------------------------------------
  public async Task UpdateDataAsync(TargetApplications targetApplications)
  {
    try
    {
      //_efDbClass.DbApplications.Update(targetApplications);
      await _efDbClass.DbApplications.Where(_ => _.Id == targetApplications.Id).ExecuteUpdateAsync(_ => _                                                                              
                                                                              .SetProperty(__ => __.AppName, __ => targetApplications.AppName)
                                                                              .SetProperty(__ => __.AppUrl, __ => targetApplications.AppUrl)
                                                                              .SetProperty(__ => __.Interval, __ => targetApplications.Interval)
      );

      var backServiceCheck = _serviceProvider.GetService<IBackCheckServices>();

      backServiceCheck!.CheckCallBacks.First(_=>_.Id == targetApplications.Id).Url = targetApplications.AppUrl;
      backServiceCheck!.CheckCallBacks.First(_ => _.Id == targetApplications.Id).AppName = targetApplications.AppName;
      backServiceCheck!.CheckCallBacks.First(_ => _.Id == targetApplications.Id).Interval = targetApplications.Interval;
    }
    catch (Exception ex)
    {
      throw;
    }
  }
}//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////