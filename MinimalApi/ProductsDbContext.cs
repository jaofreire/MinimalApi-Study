using System;


public class ProductsDbContext : DbContext
{
    public ProductsDbContext(DbContextOptions options) : base(options) { }

    public DbSet<Products> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ProductsMap());
        base.OnModelCreating(modelBuilder);
    }
}
