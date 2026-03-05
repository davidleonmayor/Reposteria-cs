using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Person> Person => Set<Person>();
    public DbSet<PersonType> PersonType => Set<PersonType>();
}
