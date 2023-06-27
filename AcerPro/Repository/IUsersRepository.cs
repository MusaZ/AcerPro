using AcerPro.EFCore;
using AcerPro.Models;
using Microsoft.EntityFrameworkCore;

namespace AcerPro.Repostory;
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
public interface IRepository<T> where T : class
{
  /// <summary>
  /// This Properties is Readonly and Keep Data which Relation with Class
  /// </summary>
  public IEnumerable<T> Repos { get; }
}
//-------------------------------------------------------------------------------------------------------------------------------
public interface IUserRepository<T> : IRepository<Users> where T : class
{
  /// <summary>
  /// This Method Add New User To User Repo and it takes one argument which User Type.
  /// </summary>
  /// <param name="user">User Type Class</param>
  /// <returns></returns>
  public Task AddUserAsync(T user);
}
//-------------------------------------------------------------------------------------------------------------------------------
public class UsersRepository : IUserRepository<Users>
{
  private readonly EfDbClass _efDbClass;
  //-----------------------------------------------------------------------------------------------------------------------------
  public UsersRepository(EfDbClass efDbClass)
  {
    _efDbClass = efDbClass;
  }
  //-----------------------------------------------------------------------------------------------------------------------------
  public IEnumerable<Users> Repos => _efDbClass.DbUsers;
  //-----------------------------------------------------------------------------------------------------------------------------
  public async Task AddUserAsync(Users user)
  {
    await _efDbClass.DbUsers.AddAsync(user);
    await _efDbClass.SaveChangesAsync();
  }
}
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////