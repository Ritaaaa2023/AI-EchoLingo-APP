using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
  public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

  public DbSet<DiaryEntry> DiaryEntries { get; set; }
  public DbSet<Tag> Tags { get; set; }
  public DbSet<DiaryEntryTag> DiaryEntryTags { get; set; }

  protected override void OnModelCreating(ModelBuilder builder)
  {
    base.OnModelCreating(builder);

    builder.Entity<DiaryEntryTag>()
        .HasKey(dt => new { dt.DiaryEntryId, dt.TagId });

    builder.Entity<DiaryEntryTag>()
        .HasOne(dt => dt.DiaryEntry)
        .WithMany(d => d.DiaryEntryTags)
        .HasForeignKey(dt => dt.DiaryEntryId);

    builder.Entity<DiaryEntryTag>()
        .HasOne(dt => dt.Tag)
        .WithMany(t => t.DiaryEntryTags)
        .HasForeignKey(dt => dt.TagId);
  }
}
