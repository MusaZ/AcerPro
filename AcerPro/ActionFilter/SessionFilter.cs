using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using StackExchange.Redis;

namespace AcerPro.ActionFilter;

public class SessionFilter : ActionFilterAttribute
{
  private readonly string? _sessionVal;
  //--------------------------------------------------------------------------------------------------------------------
  public SessionFilter(string? sessionVal)
  {
    _sessionVal = sessionVal;
  }
  //--------------------------------------------------------------------------------------------------------------------
  public void OnActionExecuting(ActionExecutingContext context)
  {
    if (_sessionVal is null)
      context.Result = new RedirectToActionResult("Login","Home",null, true);
    
    base.OnActionExecuting(context);
  }
  //--------------------------------------------------------------------------------------------------------------------
  public void OnActionExecuted(ActionExecutedContext context)
  {
    base.OnActionExecuted(context);
  }
}
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////