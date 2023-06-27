using AcerPro.Models;

namespace AcerPro.InterfacesMethods;

public interface IGetApplications
{
  public int UsrId { get; set; }
  public IEnumerable<TargetApplications> Applications { get; }
}