using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using webfoodprime.Models;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // 🔷 DbSet
    public DbSet<Food> Foods { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }
    public DbSet<Wallet> Wallets { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<ShippingFee> ShippingFees { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // =========================
        // 🔷 USER - CART (1-1)
        // =========================
        builder.Entity<Cart>()
            .HasOne(c => c.User)
            .WithOne(u => u.Cart)
            .HasForeignKey<Cart>(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Cart>()
            .HasIndex(c => c.UserId)
            .IsUnique();

        // =========================
        // 🔷 USER - WALLET (1-1)
        // =========================
        builder.Entity<Wallet>()
            .HasOne(w => w.User)
            .WithOne(u => u.Wallet)
            .HasForeignKey<Wallet>(w => w.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // =========================
        // 🔷 CART - CART ITEM (1-N)
        // =========================
        builder.Entity<CartItem>()
            .HasOne(ci => ci.Cart)
            .WithMany(c => c.CartItems)
            .HasForeignKey(ci => ci.CartId);

        builder.Entity<CartItem>()
            .HasOne(ci => ci.Food)
            .WithMany()
            .HasForeignKey(ci => ci.FoodId);

        // =========================
        // 🔷 ORDER - USER (Customer)
        // =========================
        builder.Entity<Order>()
            .HasOne(o => o.User)
            .WithMany()
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // =========================
        // 🔷 ORDER - SHIPPER
        // =========================
        builder.Entity<Order>()
            .HasOne(o => o.Shipper)
            .WithMany()
            .HasForeignKey(o => o.ShipperId)
            .OnDelete(DeleteBehavior.Restrict);

        // =========================
        // 🔷 ORDER - ADDRESS
        // =========================
        builder.Entity<Order>()
            .HasOne(o => o.Address)
            .WithMany()
            .HasForeignKey(o => o.AddressId);

        // =========================
        // 🔷 ORDER - ORDER DETAIL (1-N)
        // =========================
        builder.Entity<OrderDetail>()
            .HasOne(od => od.Order)
            .WithMany(o => o.OrderDetails)
            .HasForeignKey(od => od.OrderId);

        builder.Entity<OrderDetail>()
            .HasOne(od => od.Food)
            .WithMany()
            .HasForeignKey(od => od.FoodId);

        // =========================
        // 🔷 WALLET - TRANSACTION (1-N)
        // =========================
        builder.Entity<Transaction>()
            .HasOne(t => t.Wallet)
            .WithMany()
            .HasForeignKey(t => t.WalletId);

        // =========================
        // 🔷 DECIMAL CONFIG (TRÁNH LỖI SQL)
        // =========================
        builder.Entity<Food>()
            .Property(f => f.Price)
            .HasColumnType("decimal(18,2)");

        builder.Entity<Order>()
            .Property(o => o.TotalPrice)
            .HasColumnType("decimal(18,2)");

        builder.Entity<Order>()
            .Property(o => o.FoodTotal)
            .HasColumnType("decimal(18,2)");

        builder.Entity<Order>()
            .Property(o => o.ShippingFee)
            .HasColumnType("decimal(18,2)");

        builder.Entity<Wallet>()
            .Property(w => w.Balance)
            .HasColumnType("decimal(18,2)");
    }
}