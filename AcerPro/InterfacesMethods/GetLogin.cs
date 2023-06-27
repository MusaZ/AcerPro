using AcerPro.EFCore;
using AcerPro.Models;
using AcerPro.Repostory;

namespace AcerPro.InterfacesMethods;

public static class GetLogin
{
  public static Users? CheckUser(LoginData loginData, IEnumerable<Users> users)
  {    
    var activeUser = users.FirstOrDefault((usrs) =>
            usrs.UserName == loginData.UserName && usrs.Password == loginData.Password);

    if (activeUser is not null)
      return activeUser;

    return null;
  }
}
