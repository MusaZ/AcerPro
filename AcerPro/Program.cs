using System.Net.Mime;
using AcerPro.BackServices;
using AcerPro.CustomLogger;
using AcerPro.DataLayer;
using AcerPro.EFCore;
using AcerPro.GlobalVariables;
using AcerPro.InterfacesMethods;
using AcerPro.InterfacesMethods.CallBackServices;
using AcerPro.Middlewares;
using AcerPro.Models;
using AcerPro.Repository;
using AcerPro.Repostory;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

GlobalVariables.ConfigurationManager = builder.Configuration;
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<CheckCallBack>();
builder.Services.AddScoped<IServiceProvider, ServiceProvider>();
builder.Services.AddTransient<IConfiguration, ConfigurationManager>();
var redisConn = ConnectionMultiplexer.Connect("127.0.0.1:6379");
builder.Services.AddSingleton<IConnectionMultiplexer>((serviceProv) =>
{
  return redisConn;
});
builder.Services.AddScoped<IStreamToLogger, StreamToLogger>();
//builder.Services.AddHostedService<CheckCallBack>();
builder.Services.AddHttpContextAccessor();
builder.Logging.AddProvider(new LoggerProvider(redisConn));
builder.Services.AddSingleton<IBackCheckServices, BackCheckServices>();
builder.Services.AddScoped<IRedisDBProcess, RedisDBProcess>();
builder.Services.AddScoped<IAppRepository<TargetApplications, UpdateData, DeleteData>, ApplicaitonsRepository>();
builder.Services.AddScoped<IUserRepository<Users>, UsersRepository>();
#region PASSIVE CODE
//builder.Services.AddSession(opts =>
//{
//  opts.IdleTimeout = TimeSpan.FromMinutes(10);
//  opts.Cookie.IsEssential = true;  
//});
#endregion

builder.Services.AddHttpsRedirection(opt =>
{
  opt.HttpsPort = 44374;
});

builder.Services.AddDbContext<EfDbClass>(opts =>
{
  opts.UseSqlServer($"{builder.Configuration["ConnStr"]}");
});

builder.Services.AddLogging(logBuilder =>
{
  logBuilder.AddConsole();
  logBuilder.AddDebug();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
  //app.UseExceptionHandler("/Home/Error");
  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  app.UseHsts();
}

app.UseExceptionHandler(exceptionsHandler=>
{
  exceptionsHandler.Run(async context =>
  {
    Logger<ErrorModel> logger = new Logger<ErrorModel>(new LoggerFactory());
    var error = context.Features.Get<IExceptionHandlerFeature>();
    var errMsg = error.Error.Message;
    var errSrc = error.Error.Source;
    var errStack = error.Error.StackTrace;
    
    logger.LogError($"{errMsg}\n{errSrc}\n{errStack}");

    var locations = Directory.GetCurrentDirectory() + "/Logs/";
    string logFileName = 
            $"{DateTime.Now.Day.ToString("00")}.{DateTime.Now.Month.ToString("00")}.{DateTime.Now.Year.ToString()}.log";
    string logFilePath = String.Concat(locations, logFileName);
    string ErrTime = DateTime.Now.ToShortTimeString();
    string LogMesaj = $"[Error]-{ErrTime} : {errMsg} - {errSrc}";
    
    //if (!File.Exists(logFilePath))
    //  File.Create(logFilePath);
    
    //File.WriteAllText(logFilePath,LogMesaj);
  });
});

app.UseHttpsRedirection();
app.UseStaticFiles();
//app.UseSession();
app.UseRouting();
app.UseMiddleware <AuthenticateMiddle>();
app.UseAuthorization();
app.MapControllerRoute(
  name: "default",
  pattern: "{controller=Home}/{action=Login}/{id?}"
);

//app.UseEndpoints(endpoints =>
//{
//  endpoints.MapGet("/", (int? id, int? id2) =>
//  {
//    if (id is null && id2 is null)
//      return Results.BadRequest(new ProblemDetails() { Status = 404, Title = "" });
//    else
//      return Results.Empty;
//  });
//});

app.Run();