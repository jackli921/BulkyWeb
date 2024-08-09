using Bulky.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.General;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Bulk.DataAccess.Data;

public class ApplicationDbContext: IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
    {
    }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<ShoppingCart> ShoppingCarts { get; set; }
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public DbSet<OrderHeader> OrderHeaders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Action", DisplayOrder = 1 },
            new Category { Id = 2, Name = "SciFi", DisplayOrder = 2 },
            new Category { Id = 3, Name = "History", DisplayOrder = 3 },
            new Category { Id = 4, Name = "Anime", DisplayOrder = 4 }
        );
        
        modelBuilder.Entity<Company>().HasData(
            new Company { Id = 1, Name = "Tech Solution", StreetAddress = "150 Waterloo Ave", City = "Tech City", PostalCode = "12121", State = "IL", PhoneNumber = "123-456-789"},
            new Company { Id = 2, Name = "Vivid Books",  StreetAddress = "23 Kitchener Rd", City = "Calgary", PostalCode = "324132", State = "AB", PhoneNumber = "431-456-789"},
            new Company { Id = 3, Name = "Bulk Barn",  StreetAddress = "310 Spadina Ave", City = "Toronto", PostalCode = "M5M3G5", State = "ON", PhoneNumber = "412-454-219"},
            new Company { Id = 4, Name = "Innovative Tech", StreetAddress = "245 Silicon Blvd." ,City = "Innovation Town", PostalCode = "13131", State = "CA", PhoneNumber = "234-567-890"},
            new Company { Id = 5, Name = "Green Energy Inc.", StreetAddress = "98 Renewable Rd." ,City = "Eco City", PostalCode = "14141", State = "TX", PhoneNumber = "345-678-901"},
            new Company { Id = 6, Name = "Future Enterprises", StreetAddress = "77 Futurist Dr." ,City = "Futureville", PostalCode = "15151", State = "NY", PhoneNumber = "456-789-012"},
            new Company { Id = 7, Name = "AI Innovations", StreetAddress = "500 AI Ln." ,City = "Machine Town", PostalCode = "16161", State = "WA", PhoneNumber = "567-890-123"},
            new Company { Id = 8, Name = "HealthTech", StreetAddress = "300 Wellness Way" ,City = "Health City", PostalCode = "17171", State = "FL", PhoneNumber = "678-901-234"}
        );

        modelBuilder.Entity<Product>().HasData(new Product
            {
                Id = 1,
                Title = "Fortune of Time",
                Author = "Billy Spark",
                Description =
                    "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                ISBN = "SWD9999001",
                ListPrice = 99,
                Price = 90,
                Price50 = 85,
                Price100 = 80,
                CategoryId = 1,
                ImageUrl = ""
            },
            new Product
            {
                Id = 2,
                Title = "Dark Skies",
                Author = "Nancy Hoover",
                Description =
                    "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                ISBN = "CAW777777701",
                ListPrice = 40,
                Price = 30,
                Price50 = 25,
                Price100 = 20,
                CategoryId = 1,
                ImageUrl = ""
            },
            new Product
            {
                Id = 3,
                Title = "Vanish in the Sunset",
                Author = "Julian Button",
                Description =
                    "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                ISBN = "RITO5555501",
                ListPrice = 55,
                Price = 50,
                Price50 = 40,
                Price100 = 35,
                CategoryId = 2,
                ImageUrl = ""
            },
            new Product
            {
                Id = 4,
                Title = "Cotton Candy",
                Author = "Abby Muscles",
                Description =
                    "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                ISBN = "WS3333333301",
                ListPrice = 70,
                Price = 65,
                Price50 = 60,
                Price100 = 55,
                CategoryId = 2,
                ImageUrl = ""
            },
            new Product
            {
                Id = 5,
                Title = "Rock in the Ocean",
                Author = "Ron Parker",
                Description =
                    "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                ISBN = "SOTJ1111111101",
                ListPrice = 30,
                Price = 27,
                Price50 = 25,
                Price100 = 20,
                CategoryId = 3,
                ImageUrl = ""
            },
            new Product
            {
                Id = 6,
                Title = "Leaves and Wonders",
                Author = "Laura Phantom",
                Description =
                    "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                ISBN = "FOT000000001",
                ListPrice = 25,
                Price = 23,
                Price50 = 22,
                Price100 = 20,
                CategoryId = 3,
                ImageUrl = ""
            }
        );
    }
}