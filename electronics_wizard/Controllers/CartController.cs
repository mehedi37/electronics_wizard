using electronics_wizard.Data;
using electronics_wizard.Services;
using electronics_wizard.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace electronics_wizard.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly AppDbContext _context;
        private readonly CartServices _cartServices;
        private readonly ElectronicServices _electronicServices;

        public CartController(AppDbContext context, CartServices cartServices, ElectronicServices electronicServices)
        {
            _context = context;
            _cartServices = cartServices;
            _electronicServices = electronicServices;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = await _cartServices.CartByUserIdAsync(userId);

            if (cart == null || !cart.CartItems.Any())
            {
                ViewBag.Message = "Your cart is empty.";
                return View(new CartViewModel());
            }

            var cartViewModel = new CartViewModel
            {
                CartId = cart.CartId,
                CartItems = cart.CartItems.ToList(),
                TotalPrice = cart.CartItems.Sum(cd => cd.Price * cd.Quantity)
            };

            return View(cartViewModel);
        }


        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var product = await _context.Electronics.FindAsync(productId);

            if (product == null || product.Stock < quantity)
            {
                return BadRequest("Insufficient stock or product not found.");
            }

            await _cartServices.AddToCartAsync(userId, productId, quantity);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCart(int cartItemId, int quantity)
        {
            await _cartServices.UpdateCartAsync(cartItemId, quantity);
            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int cartItemsId)
        {
            await _cartServices.RemoveFromCartAsync(cartItemsId);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> ClearCart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _cartServices.ClearCartAsync(userId);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Purchase()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = await _cartServices.CartByUserIdAsync(userId);

            if (cart == null || !cart.CartItems.Any())
            {
                return BadRequest("Your cart is empty.");
            }

            foreach (var cartDetail in cart.CartItems)
            {
                var product = await _context.Electronics.FindAsync(cartDetail.ElectronicId);
                if (product == null || product.Stock < cartDetail.Quantity)
                {
                    return BadRequest("Insufficient stock for product: " + product?.ElectronicName);
                }

                product.Stock -= cartDetail.Quantity;
                await _electronicServices.UpdateItemAsync(product);
            }

            await _cartServices.PurchaseCartAsync(userId);
            return RedirectToAction("Index");
        }

    }
}
