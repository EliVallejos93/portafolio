using Microsoft.EntityFrameworkCore;

namespace CHALLENGEIntuit_Back
{
    public class APIDbContext : DbContext
    {
        public APIDbContext(DbContextOptions<APIDbContext> options) : base(options)
        {
        }

        public DbSet<Clientes> Clientes { get; set; }
    }
}
