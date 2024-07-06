using Microsoft.EntityFrameworkCore;

namespace BulkyWeb.Controllers.Data;

public class ApplicationDbContext: DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
    {
    }
}