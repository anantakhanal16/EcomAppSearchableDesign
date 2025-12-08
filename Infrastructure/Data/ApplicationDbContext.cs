using Core.Entities;
using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : IdentityDbContext<User>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetails> OrderDetails { get; set; }
    public DbSet<Cart> Cart { get; set; }
    public DbSet<CartItem> CartItem { get; set; }
    public DbSet<ProductReview> ProductReviews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>().HasOne(p => p.Supplier).WithMany(s => s.Products).HasForeignKey(p => p.SupplierID).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<OrderDetails>().HasOne(od => od.Product).WithMany(p => p.OrderDetails).HasForeignKey(od => od.ProductID).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<OrderDetails>().HasOne(od => od.Order).WithMany(o => o.OrderDetails).HasForeignKey(od => od.OrderID).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Cart>().HasMany(c => c.CartItems).WithOne(ci => ci.Cart).HasForeignKey(ci => ci.CartID).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<CartItem>().HasOne(ci => ci.Product).WithMany().HasForeignKey(ci => ci.ProductID).OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<ProductReview>().HasOne(r => r.Product).WithMany(p => p.ProductReviews).HasForeignKey(r => r.ProductId).OnDelete(DeleteBehavior.Cascade);
    }
}