using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Person> Person => Set<Person>();
    public DbSet<PersonType> PersonType => Set<PersonType>();
    public DbSet<Category> Category => Set<Category>();
    public DbSet<Product> Product => Set<Product>();

    public DbSet<Sale> Sale => Set<Sale>();
    public DbSet<SaleDetail> SaleDetail => Set<SaleDetail>();
    public DbSet<SaleParticipant> SaleParticipant => Set<SaleParticipant>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SaleDetail>()
            .HasOne(d => d.Sale)
            .WithMany(s => s.Details)
            .HasForeignKey(d => d.SaleId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SaleDetail>()
            .HasOne(d => d.Product)
            .WithMany()
            .HasForeignKey(d => d.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SaleParticipant>()
            .HasOne(sp => sp.Sale)
            .WithMany(s => s.Participants)
            .HasForeignKey(sp => sp.SaleId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SaleParticipant>()
            .HasOne(sp => sp.Person)
            .WithMany(p => p.SaleParticipations)
            .HasForeignKey(sp => sp.PersonId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SaleParticipant>()
            .HasIndex(sp => new { sp.SaleId, sp.PersonId, sp.Role })
            .IsUnique();
    }
}
