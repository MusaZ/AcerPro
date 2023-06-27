using AcerPro.EFCore;
using AcerPro.Models;

namespace AcerPro.InterfacesMethods;

public class GetApplicaitons : IGetApplications
{
  private readonly EfDbClass _efDbClass;

  public GetApplicaitons(EfDbClass efDbClass)
  {
    _efDbClass = efDbClass;
  }
  public int UsrId { get; set; }

  public IEnumerable<TargetApplications> Applications => _efDbClass.DbApplications.Where(usrs => usrs.Id == UsrId);
}