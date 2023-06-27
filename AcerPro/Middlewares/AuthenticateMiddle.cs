using StackExchange.Redis;

namespace AcerPro.Middlewares;

public class AuthenticateMiddle
{
  private RequestDelegate _requestDelegate;
  private readonly IServiceProvider _serviceProvider;

  public AuthenticateMiddle(RequestDelegate requestDelegate, IServiceProvider serviceProvider)
  {
    _requestDelegate = requestDelegate;
    _serviceProvider = serviceProvider;
  }

  public async Task Invoke(HttpContext context)
  {
    var redDb = _serviceProvider.GetService<IConnectionMultiplexer>();
    var redisDB = redDb.GetDatabase();
    var isSess = redisDB.StringGet("UserSession");
    var userCookie = context.Request.Cookies.Keys.FirstOrDefault(_ => _ == "UserCookie");

    if (userCookie is null && !context.Request.Path.Value!.Contains("Login", StringComparison.InvariantCultureIgnoreCase) && !context.Request.Path.Value.ToLower().Contains("adduser", StringComparison.OrdinalIgnoreCase))
      if(context.Request.Path.Value.ToLower().Contains("home", StringComparison.InvariantCultureIgnoreCase))
        context.Response.Redirect("Login");
      else
        context.Response.Redirect("Home/Login");
    else      
        await _requestDelegate(context);      
  }
}