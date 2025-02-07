using dotnet_notely.Data.Configuration;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace dotnet_notely.Data;

public class NotelyDbContext: IdentityDbContext<ApiUser>
{
    public NotelyDbContext(DbContextOptions options) : base(options)
    {
        
    }
    
    public DbSet<Note> Notes { get; set; }
    public DbSet<NoteShare> NoteShares { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new NoteConfiguration());
        modelBuilder.ApplyConfiguration(new UserNoteConfiguration());
        
    }
}