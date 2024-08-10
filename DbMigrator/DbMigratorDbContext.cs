using Microsoft.EntityFrameworkCore;

public class DbMigratorDbContext : DbContext
{
    public DbMigratorDbContext(DbContextOptions<DbMigratorDbContext> options) : base(options)
    {
    }
}