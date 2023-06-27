using System.Diagnostics;
using AcerPro.ActionFilter;
using AcerPro.BackServices;
using AcerPro.EFCore;
using AcerPro.InterfacesMethods;
using Microsoft.AspNetCore.Mvc;
using AcerPro.Models;
using AcerPro.Repostory;
using Microsoft.EntityFrameworkCore;
using Sio = System.IO;
using StackExchange.Redis;
using System.Text;
using System.Linq;
using Newtonsoft.Json;
using CSharpVitamins;
using AcerPro.DataLayer;
using AcerPro.Repository;
using AcerPro.InterfacesMethods.CallBackServices;

namespace AcerPro.Controllers;

public class HomeController : Controller
{
  #region CONSTANT VARIABLE
  private readonly string CookieName = "UserCookie";
  private readonly string UserId = "UserId";
  private readonly string UserMail = "UserMail";
  public const int SessionTime = 10;
  private readonly TimeSpan SessionTimeSpan = TimeSpan.FromMinutes(SessionTime);
  #endregion
  
  //------------------------------------------------------------------------------------------------------------------------
  
  #region DI VARIABLE
  private readonly ILogger<HomeController> _logger;
  private readonly IServiceProvider _serviceProvider;
  private readonly EfDbClass _efDbClass;
  private readonly IConnectionMultiplexer _connectionMultiplexer;
  private readonly IHttpContextAccessor _contextAccessor;
  private IBackCheckServices _backCheckServices;
  #endregion

  //------------------------------------------------------------------------------------------------------------------------
  public HomeController(ILogger<HomeController> logger, IServiceProvider serviceProvider, EfDbClass efDbClass, IConnectionMultiplexer connectionMultiplexer, IHttpContextAccessor contextAccessor, IBackCheckServices backCheckServices)
  {
    _logger = logger;
    _serviceProvider = serviceProvider;
    _efDbClass = efDbClass;
    _connectionMultiplexer = connectionMultiplexer;
    _contextAccessor = contextAccessor;
    _backCheckServices = backCheckServices;

    GlobalVariables.GlobalVariables.connectionMultiplexer = connectionMultiplexer;    
  }
  //------------------------------------------------------------------------------------------------------------------------
  [HttpGet("{controller}/Login")]
  [HttpGet("{controller}")]
  [HttpGet("/")]
  public IActionResult Login()
  {
    return View("Index");
  }
  //------------------------------------------------------------------------------------------------------------------------
  [HttpPost("{controller}/Login")]
  public IActionResult Login(LoginData loginData)
  {
    if (!ModelState.IsValid)
      return View("Index", loginData);

    IRepository<Users> usersRepository = new UsersRepository(_efDbClass);
    var IsUserValid = GetLogin.CheckUser(loginData, usersRepository.Repos);
    
    if (IsUserValid is not null)
    {
      #region Save Data To TempData
      TempData[UserId] = IsUserValid.Id;
      TempData[UserMail] = IsUserValid.EMail;
      TempData.Save();
      #endregion

      var userGuide = Guid.NewGuid().ToString();
      _contextAccessor.HttpContext!.Response.Cookies.Append(CookieName, userGuide, new CookieOptions() 
      { 
        Expires = DateTime.Now.AddMinutes(SessionTime)
      });

      loginData.IsSuccess = true;

      #region Get RedisDB Process Class via ServiceProvider for Save Session Datas to Redis
      var redisDb = _serviceProvider.GetService<IRedisDBProcess>();
      redisDb.AddDataAsync("UserSession", Guid.NewGuid().ToString(), SessionTimeSpan);
      redisDb.AddDataAsync("UserID", IsUserValid.Id, SessionTimeSpan);
      #endregion

      LoadCheckCallback loadCheckCallback = new LoadCheckCallback(_serviceProvider, _efDbClass, IsUserValid.EMail, 
                                                                  IsUserValid.Id, _backCheckServices);

      return RedirectToAction("MainPage", "Home");
    }
    else
    {
      loginData.IsSuccess = false;
      return View("Index", loginData);
    }
  }
  //------------------------------------------------------------------------------------------------------------------------
  public IActionResult MainPage()
  {
    IRepository<TargetApplications> targetApps = new ApplicaitonsRepository(_efDbClass, _serviceProvider);
    var AppsOfUser = targetApps.Repos.Where(apps => apps.UserId == Convert.ToInt32(TempData.Peek(UserId)));
    return View("MainPage", AppsOfUser);
  }
  
  //------------------------------------------------------------------------------------------------------------------------
  [HttpGet("{controller}/AddUser")]
  public IActionResult AddUser()
  {
    return View("AddUser");
  }
  //------------------------------------------------------------------------------------------------------------------------
  [HttpPost("{controller}/AddUser")]
  public async Task<IActionResult> AddUser(Users users)
  {
    if(!ModelState.IsValid)
      return View("AddUser", users);

    var usrr = _efDbClass.DbUsers.FirstOrDefault(usrs => usrs.UserName == users.UserName);
    if (usrr is null)
    {
      var userRepo = _serviceProvider.GetService<IUserRepository<Users>>();
      await userRepo!.AddUserAsync(users);
    }    

    return Redirect("AddUser");
  }
  //------------------------------------------------------------------------------------------------------------------------
  [HttpGet("{controller}/AddApplications")]
  public IActionResult AddApplications()
  {
    return View("AddApplications");
  }
  
  //------------------------------------------------------------------------------------------------------------------------
  [HttpPost("{controller}/AddApplications")]
  public async Task<IActionResult> AddApplications(TargetApplications targetApplications)
  {
    if (!ModelState.IsValid)
      return View("AddApplications", targetApplications);

    #region Checks App Name Against To Whether There Is Or Not
    IRepository<TargetApplications> apps = new ApplicaitonsRepository(_efDbClass, _serviceProvider);
    var isThereApp = apps.Repos.FirstOrDefault(ap => ap.AppName == targetApplications.AppName);
    if (isThereApp is null)
    {
      var appRepository = _serviceProvider.GetService<IAppRepository<TargetApplications, UpdateData, DeleteData>>();
      await appRepository!.AddApplicationAsync(targetApplications, Convert.ToInt32(TempData.Peek(UserId)), 
                                                TempData.Peek(UserMail)!.ToString()! );

      return Redirect("AddApplications");
    }
    #endregion

    return View("AddApplications", targetApplications);
  }
  //------------------------------------------------------------------------------------------------------------------------
  [HttpDelete("{controller}/DeleteApps")]
  public async Task<IActionResult> DeleteApps([FromBody]DeleteData deleteData)
  {
    int userId = Convert.ToInt32(TempData.Peek(UserId));
    var appRepo = _serviceProvider.GetService<IAppRepository<TargetApplications, UpdateData, DeleteData>>();
    await appRepo!.DeleteDataAsync(userId, deleteData);
    
    return RedirectToAction("MainPage", "Home");
  }
  //------------------------------------------------------------------------------------------------------------------------
  [HttpPatch("{controller}/UpdateApps")]
  public async Task<IActionResult> UpdateApps([FromBody]UpdateData updateData)
  {
    #region --DISABLE UPDATE CODE--
    //_efDbClass.DbApplications.Where(apps => apps.Id == updateData.Id).ExecuteUpdate(setprop =>
    //                                                                                  setprop
    //                                                                                  .SetProperty(target => target.AppName, updateData.AppName)
    //                                                                                  .SetProperty(target => target.AppUrl, updateData.AppUrl)
    //                                                                                  .SetProperty(target => target.Interval, updateData.Interval));
    //_efDbClass.SaveChanges();
    #endregion        

    var appRepo = _serviceProvider.GetService<IAppRepository<TargetApplications, UpdateData, DeleteData>>();
    await appRepo!.UpdateDataAsync(updateData);
    
    return RedirectToAction("MainPage", "Home");
  }
  //------------------------------------------------------------------------------------------------------------------------
  [HttpGet("{controller}/Logout")]
  public IActionResult Logout()
  {
    var userId = Convert.ToInt32(TempData.Peek(UserId));

    #region Remove Apps In Dynamic List
    var backCheckService = _serviceProvider.GetService<IBackCheckServices>();
    backCheckService!.RemoveBackCheckService(userId);
    #endregion

    #region Remove Apps In Redis
    var redisDb = _serviceProvider.GetService<IRedisDBProcess>();
    redisDb!.RemoveKeyDataAsync("UserSession");
    redisDb!.RemoveAllHashAsync("Apps");
    #endregion    

    TempData.Clear();

    _contextAccessor.HttpContext!.Response.Cookies.Delete(CookieName);

    return Redirect("Login");
  }
  //------------------------------------------------------------------------------------------------------------------------
  [HttpGet("{controller}/Logs")]
  public IActionResult Logs()
  {   
    try
    {
      string logFileName = DateTime.Now.ToString("dd.MM.yyyy");
      string logFile = $"{Directory.GetCurrentDirectory()}/Logs/{logFileName}.log";
      string logVal = string.Empty;
      if (Sio::File.Exists(logFile))
      {
        using FileStream fileStream = new FileStream(logFile, FileMode.Open);
        byte[] buffer = new byte[fileStream.Length];
        Span<byte> bytes = buffer.AsSpan<byte>();
        fileStream.Read(bytes);

        logVal = Encoding.UTF8.GetString(bytes);
      }

      IStreamToLogger? streamToLogger = _serviceProvider.GetService<IStreamToLogger>();
      if (streamToLogger is not null)
      {
        streamToLogger.RawString = logVal;
        var userLogs = streamToLogger.ConvertToLogger();
        return View("Logs", userLogs);
      }
      
      return View("Logs", Enumerable.Empty<Logger>());
    }
    catch (Exception ex)
    {      
      return View("Logs");
    }    
  }
  //------------------------------------------------------------------------------------------------------------------------  
  [HttpGet("{controller}/GetStates")]
  public async Task<string> GetAppStateAsync()
  {
    List<AppAndState> appAndStates = new List<AppAndState>();

    var redisDb = _connectionMultiplexer.GetDatabase();
    var appsHashes = await redisDb.HashGetAllAsync("Apps");
    foreach(var app in appsHashes)
    {
      appAndStates.Add(new AppAndState()
      {
        AppName = app.Name,
        AppState = Convert.ToInt32(app.Value)
      });
    }

    string retData = JsonConvert.SerializeObject(appAndStates);
    
    return await Task.FromResult(retData);
  }
  //------------------------------------------------------------------------------------------------------------------------
  [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
  public IActionResult Error()
  {
    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
  }
}