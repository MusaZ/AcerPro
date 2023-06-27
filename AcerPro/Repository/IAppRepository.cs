using AcerPro.Repostory;

namespace AcerPro.Repository;

public interface IAppRepository<T, T1, T2> : IRepository<T> where T: class where T1: class where T2 : class
{
  public Task AddApplicationAsync(T t, int userId, string userMail);
  public Task UpdateDataAsync(T t);
  public Task UpdateDataAsync(T1 t1);
  public Task DeleteDataAsync(T t);
  public Task DeleteDataAsync(int usrId, T2 t2);
}
