using electronics_wizard.Data;
using electronics_wizard.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace electronics_wizard.Services
{
    public class CustomerServices
    {
        private readonly AppDbContext _context;

        public CustomerServices(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<CustomerViewModel>> CustomersBySellerIdAsync(string sellerId)
        {
            // Fetch the necessary data without performing the aggregation
            var cartData = await _context.Carts
                .Where(c => c.IsPurchased && c.CartItems != null && c.CartItems.Any(cd => cd.Electronics != null && cd.Electronics.UserId == sellerId))
                .Include(c => c.CartItems)
                .ThenInclude(cd => cd.Electronics)
                .Include(c => c.AppUserUser)
                .ToListAsync();

            // Perform the aggregation in memory
            var customers = cartData
                .GroupBy(c => c.UserId)
                .Select(g => new CustomerViewModel
                {
                    CustomerName = g.First().AppUserUser?.Name ?? "Unknown",
                    TotalPrice = g.Sum(c => c.CartItems.Sum(cd => cd.Price * cd.Quantity))
                })
                .ToList();

            return customers ?? new List<CustomerViewModel>();
        }
    }
}
