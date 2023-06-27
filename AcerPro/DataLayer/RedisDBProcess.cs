using AcerPro.InterfacesMethods.CallBackServices;
using AcerPro.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using StackExchange.Redis;
using System.Runtime.CompilerServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AcerPro.DataLayer;

////////////////////////////////////////////////////////////////////////////////////////////////////////////
public interface IRedisDBProcess
{  
  /// <summary>
  /// This Function Add a Data To Redis DB as String
  /// </summary>
  /// <param name="key">Key that will relation with will save data in RedisDB</param>
  /// <param name="Data">Data that desired value to saved in RedisDB</param>
  /// <param name="sessionTime">Time that how long of data keep in RedisDB</param>
  /// <returns></returns>
  public Task AddDataAsync(string key, object Data, TimeSpan sessionTime);

  /// <summary>
  /// This Function RemoveString Data from RedisDB
  /// </summary>
  /// <param name="key">Key that will Remove in RedisDB</param>  
  /// <returns></returns>
  public Task RemoveKeyDataAsync(string key);

  /// <summary>
  /// This Function RemoveString Data from RedisDB
  /// </summary>
  /// <param name="key">Key that will Remove in RedisDB</param>  
  /// <param name="item">Item that will remove from list in RedisDB</param>
  /// <returns></returns>
  public Task RemoveListDataAsync(string key, string item);

  /// <summary>
  /// This Method Delete given HashFields belong to Key which given.
  /// </summary>
  /// <param name="key">Key which will make remove in the Redis.</param>
  /// <param name="hashfield">HashFields which will delete Hashes in Given Hash Key.</param>
  /// <returns></returns>
  public Task RemoveHashDataAsync(string key, IEnumerable<string> hashfield);

  /// <summary>
  /// This Method Delete All HashFields belong to Key which given.
  /// </summary>
  /// <param name="key">Key which will make remove in the Redis.</param>  
  /// <returns></returns>
  public Task RemoveAllHashAsync(string key);
}
//---------------------------------------------------------------------------------------------------------
public class RedisDBProcess : IRedisDBProcess
{  
  private readonly IDatabase _database;
  private readonly IServiceProvider _serviceProvider;
  //---------------------------------------------------------------------------------------------------------
  public RedisDBProcess(IServiceProvider serviceProvider)
  {
    _database = serviceProvider.GetService<IConnectionMultiplexer>()!.GetDatabase();
    _serviceProvider = serviceProvider;
  }
  //---------------------------------------------------------------------------------------------------------
  public async Task AddDataAsync(string key, object Data, TimeSpan sessionTime)
  {    
    await _database.StringSetAsync(key, Data.ToString(), sessionTime);
  }
  //---------------------------------------------------------------------------------------------------------
  public async Task RemoveKeyDataAsync(string key)
  {
    await _database.KeyDeleteAsync(key);
  }
  //---------------------------------------------------------------------------------------------------------
  public async Task RemoveListDataAsync(string key, string item)
  {
    await _database.ListRemoveAsync(key, item);
  }
  //---------------------------------------------------------------------------------------------------------
  public async Task RemoveHashDataAsync(string key, IEnumerable<string> hashfield)
  {
    for (int a = 0; a < hashfield.Count(); a++)
    {
      await _database.HashDeleteAsync(key, hashfield.ElementAt(a));
    }
  }
  //---------------------------------------------------------------------------------------------------------
  public async Task RemoveAllHashAsync(string key)
  {
    var hashes = await _database.HashGetAllAsync(key);
    for (int a = 0; a < hashes.Length; a++)
    {
      await _database.HashDeleteAsync(key, hashes[a].Name);
    }
  }
}
////////////////////////////////////////////////////////////////////////////////////////////////////////////