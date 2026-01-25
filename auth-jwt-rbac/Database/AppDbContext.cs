using auth_jwt_rbac.Entities;
using Microsoft.EntityFrameworkCore;

namespace auth_jwt_rbac.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<User> users { get; set; }
    }
}
