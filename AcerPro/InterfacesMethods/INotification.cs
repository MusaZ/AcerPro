namespace AcerPro.InterfacesMethods;

public interface INotification
{
  public IEnumerable<string> Destinations { get; set; }
  public Task DoNotificationAsync();
}