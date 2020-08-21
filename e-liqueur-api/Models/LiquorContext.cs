using Microsoft.EntityFrameworkCore;
using e_liqueur.Models;

namespace e_liqueur.Models
{
    public class LiquorContext : DbContext
    {
        public LiquorContext(DbContextOptions<LiquorContext> options)
            : base(options)
        {
        }
        
        public DbSet<Store> Stores { get; set; }
        public DbSet<StockItem> Stock { get; set; }
    }
}