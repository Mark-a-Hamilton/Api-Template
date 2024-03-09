using Microsoft.EntityFrameworkCore;
using WebAPI.Library.Models;

namespace WebAPI.Library.Contexts;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {

    }
    
    public DbSet<DataModel> Log { get; set; }
}