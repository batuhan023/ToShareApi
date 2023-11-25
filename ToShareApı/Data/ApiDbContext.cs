using Microsoft.EntityFrameworkCore;
using ToShareApı.Models;

namespace ToShareApı.Data
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Apply> Apply { get; set; }
    }
}
