using FileDump.Api.Data.Postgres.Configurations;
using FileDump.Api.Data.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace FileDump.Api.Data.Postgres;

public class EfContext(DbContextOptions<EfContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        
        base.OnModelCreating(modelBuilder);
    }
}