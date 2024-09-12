using electronics_wizard.Data;
using electronics_wizard.Models;
using electronics_wizard.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace electronics_wizard.Services
{
    public class ElectronicServices
    {
        private readonly AppDbContext _context;

        public ElectronicServices(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Electronics>> ItemsForSaleByUserIdAsync(string userId)
        {
            return await _context.Electronics.Where(p => p.UserId == userId).ToListAsync();
        }

        public async Task<Electronics?> ItemByIdAsync(int id)
        {
            return await _context.Electronics.FindAsync(id);
        }

        public async Task AddItemAsync(Electronics product)
        {
            _context.Electronics.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateItemAsync(Electronics product)
        {
            _context.Electronics.Update(product);
            await _context.SaveChangesAsync();
        }


        public async Task DeleteItemAsync(int id)
        {
            var product = await _context.Electronics.FindAsync(id);
            if (product != null)
            {
                _context.Electronics.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ElectronicDetailsViewModel?> GetElectronicDetailsAsync(int id)
        {
            var electronic = await _context.Electronics.FindAsync(id);
            if (electronic == null)
            {
                return null;
            }

            var otherElectronics = await _context.Electronics
                .Where(e => e.UserId == electronic.UserId && e.ElectronicId != id)
                .ToListAsync();

            return new ElectronicDetailsViewModel
            {
                Electronics = electronic,
                OtherElectronics = otherElectronics
            };
        }
    }
}
