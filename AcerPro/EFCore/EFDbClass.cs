using AcerPro.Models;
using Microsoft.EntityFrameworkCore;

namespace AcerPro.EFCore;

public class EfDbClass : DbContext
{
  public DbSet<Users> DbUsers { get; set; }
  public DbSet<TargetApplications> DbApplications { get; set; }
  private readonly string _connStr;

  public EfDbClass(DbContextOptions<EfDbClass> options) : base(options) { }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<Users>().HasIndex(usrs => usrs.UserName).IsUnique(true);
    modelBuilder.Entity<TargetApplications>().Property(apps => apps.AppName).HasDefaultValue("App X");
  }
}