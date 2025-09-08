using System.Net;
using FileDump.Api.Data.Postgres.Configurations;
using FileDump.Api.Data.Postgres.Entities;
using Microsoft.EntityFrameworkCore;
using File = FileDump.Api.Data.Postgres.Entities.File;

namespace FileDump.Api.Data.Postgres;

public class EfContext(DbContextOptions<EfContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    
    public DbSet<File> Files { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new FileConfiguration());
        
        base.OnModelCreating(modelBuilder);
    }
}