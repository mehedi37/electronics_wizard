using electronics_wizard.Areas.Identity.Data;
using electronics_wizard.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace electronics_wizard.Data
{
    public class AppDbContext : IdentityDbContext<AppUserUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Electronics> Electronics { get; set; }
        public DbSet<CartModel> Carts { get; set; }
        public DbSet<CartItemsModel> CartItems { get; set; }
    }
}
