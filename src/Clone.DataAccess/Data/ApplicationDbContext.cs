using Clone.Models;
using Clone.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Clone.DataAccess.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Company> Companies { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<DocumentType> DocumentTypes { get; set; }
    public DbSet<OrderHeader> OrderHeaders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        modelBuilder.Entity<IdentityRole>().HasData(
            new IdentityRole
            {
                Id = "79d95740-e176-49ee-898a-378ab6497371",
                ConcurrencyStamp = "3604fc1d-cd6a-46ad-ace4-9b5f8e03f43b",
                Name = RD.RoleAdmin,
                NormalizedName = RD.RoleAdmin.ToUpper()
            },
            new IdentityRole
            {
                Id = "638aaa3e-2bfe-4a00-8230-3af7be3893af",
                ConcurrencyStamp = "6f2c2a7e-9f9b-4a0d-9f7f-2a1b3c4d5e6f",
                Name = RD.RoleCustomer,
                NormalizedName = RD.RoleCustomer.ToUpper()
            },
            new IdentityRole
            {
                Id = "1bfeff8a-8274-48c3-859a-2288aaaf67ce",
                ConcurrencyStamp = "b2e5c1d4-7a9f-4d2c-8f1e-3a4b5c6d7e8f",
                Name = RD.RoleEmployee,
                NormalizedName = RD.RoleEmployee.ToUpper()
            },
            new IdentityRole
            {
                Id = "2d79532f-97dd-44e4-9824-c0b1aac5da9c",
                ConcurrencyStamp = "02d86d56-8e63-4d2e-92f8-81b154ba0532",
                Name = RD.RoleSupplier,
                NormalizedName = RD.RoleSupplier.ToUpper()
            });
        modelBuilder.Entity<Company>()
            .Property<uint>("xmin")
            .IsConcurrencyToken()
            .ValueGeneratedOnAddOrUpdate();
        modelBuilder.Entity<Document>()
            .Property<uint>("xmin")
            .IsConcurrencyToken()
            .ValueGeneratedOnAddOrUpdate();
        modelBuilder.Entity<DocumentType>()
            .Property<uint>("xmin")
            .IsConcurrencyToken()
            .ValueGeneratedOnAddOrUpdate();
        modelBuilder.Entity<OrderHeader>()
            .Property<uint>("xmin")
            .IsConcurrencyToken()
            .ValueGeneratedOnAddOrUpdate();
        modelBuilder.Entity<OrderDetail>()
            .Property<uint>("xmin")
            .IsConcurrencyToken()
            .ValueGeneratedOnAddOrUpdate();
    }
}


