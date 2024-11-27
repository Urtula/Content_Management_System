using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using CMS.Domain.Entities;


namespace CMS.Infrastructure.Data
{
    public class CMSDbContext : DbContext
    {
        public CMSDbContext(DbContextOptions<CMSDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Content> Contents { get; set; }
        public DbSet<Variant> Variants { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Content>()
                .HasOne<Category>()
                .WithMany()
                .HasForeignKey(c => c.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Variant>()
                .HasOne<Content>()
                .WithMany()
                .HasForeignKey(v => v.ContentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure User-Category Many-to-Many relationship
            modelBuilder.Entity<User>()
                .HasMany(u => u.Categories)
                .WithMany(c => c.Users)
                .UsingEntity(j => j.ToTable("UserCategories")); // Join table for Many-to-Many relationship

            modelBuilder.Entity<Category>()
                .Property(c => c.CategoryName)
                .IsRequired();
        }
    }
    public class CMSDbContextFactory : IDesignTimeDbContextFactory<CMSDbContext>
    {
        public CMSDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<CMSDbContext>();
            optionsBuilder.UseSqlServer(connectionString, b => b.MigrationsAssembly("CMS"));

            return new CMSDbContext(optionsBuilder.Options);
        }
    }
    
}
