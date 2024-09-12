using electronics_wizard.Areas.Identity.Data;
using electronics_wizard.Data;
using electronics_wizard.Models;
using electronics_wizard.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace electronics_wizard.Services
{
    public class CartServices
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUserUser> _userManager;

        public CartServices(AppDbContext context, UserManager<AppUserUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<CartViewModel?> CartItemsAsync(AppUserUser user)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(cd => cd.Electronics)
                .FirstOrDefaultAsync(c => c.UserId == user.Id && !c.IsPurchased);

            if (cart == null || cart.CartItems == null)
            {
                return null;
            }

            return new CartViewModel
            {
                CartId = cart.CartId,
                TotalPrice = cart.CartItems.Sum(ci => ci.Price * ci.Quantity),
                CartItems = cart.CartItems.ToList()
            };
        }

        public async Task<CartViewModel?> CartByUserIdAsync(string userId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(cd => cd.Electronics)
                .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsPurchased);

            if (cart == null || cart.CartItems == null)
            {
                return null;
            }

            return new CartViewModel
            {
                CartId = cart.CartId,
                TotalPrice = cart.CartItems.Sum(ci => ci.Price * ci.Quantity),
                CartItems = cart.CartItems.ToList()
            };
        }

        public async Task AddToCartAsync(string userId, int productId, int quantity)
        {
            var cart = await CartByUserIdAsync(userId);

            if (cart == null)
            {
                var newCart = new CartModel
                {
                    CartId = new int(),
                    UserId = userId,
                    IsPurchased = false
                };
                _context.Carts.Add(newCart);
                await _context.SaveChangesAsync();
                cart = await CartByUserIdAsync(userId);
            }

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(cd => cd.CartId == cart.CartId && cd.ElectronicId == productId);

            var product = await _context.Electronics.FindAsync(productId);
            if (product == null)
            {
                throw new Exception("Product not found");
            }

            if (cartItem == null)
            {
                if (quantity > product.Stock)
                {
                    quantity = product.Stock;
                }

                cartItem = new CartItemsModel
                {
                    CartItemsId = new int(),
                    CartId = cart.CartId,
                    ElectronicId = productId,
                    Quantity = quantity,
                    Price = product.Price
                };
                _context.CartItems.Add(cartItem);
            }
            else
            {
                var newQuantity = cartItem.Quantity + quantity;
                if (newQuantity > product.Stock)
                {
                    newQuantity = product.Stock;
                }
                cartItem.Quantity = newQuantity;
                _context.CartItems.Update(cartItem);
            }

            await _context.SaveChangesAsync();
        }


        public async Task UpdateCartAsync(int cartItemId, int quantity)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem != null)
            {
                var product = await _context.Electronics.FindAsync(cartItem.ElectronicId);
                if (product == null)
                {
                    throw new Exception("Product not found");
                }

                if (quantity < 1)
                {
                    quantity = 1;
                }
                else if (quantity > product.Stock)
                {
                    quantity = product.Stock;
                }

                cartItem.Quantity = quantity;
                _context.CartItems.Update(cartItem);
                await _context.SaveChangesAsync();
            }
        }


        public async Task RemoveFromCartAsync(int cartItemId)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();

                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .FirstOrDefaultAsync(c => c.CartId == cartItem.CartId);

                if (cart != null && !cart.CartItems.Any())
                {
                    _context.Carts.Remove(cart);
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task<int> CartItemsCountAsync(string userId)
        {
            var cart = await CartByUserIdAsync(userId);
            return cart?.CartItems.Count ?? 0;
        }

        public async Task ClearCartAsync(string userId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsPurchased);

            if (cart != null)
            {
                _context.CartItems.RemoveRange(cart.CartItems);
                _context.Carts.Remove(cart);
                await _context.SaveChangesAsync();
            }
        }

        public async Task PurchaseCartAsync(string userId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsPurchased);

            if (cart != null)
            {
                cart.IsPurchased = true;
                _context.Carts.Update(cart);
                await _context.SaveChangesAsync();
            }
        }
    }
}
