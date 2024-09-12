using electronics_wizard.Areas.Identity.Data;
using electronics_wizard.Data;
using electronics_wizard.Models;
using electronics_wizard.Services;
using electronics_wizard.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace electronics_wizard.Controllers
{
    [Authorize]
    public class SellController : Controller
    {
        private readonly ILogger<SellController> _logger;
        private readonly AppDbContext _context;
        private readonly UserManager<AppUserUser> _userManager;
        private readonly ElectronicServices _electronicServices;
        private readonly CustomerServices _customerServices;

        public SellController(ILogger<SellController> logger, AppDbContext context, UserManager<AppUserUser> userManager, ElectronicServices electronicServices, CustomerServices customerServices)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _electronicServices = electronicServices;
            _customerServices = customerServices;
        }

        public async Task<IActionResult> MyElectronics()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            var itemsForSale = await _electronicServices.ItemsForSaleByUserIdAsync(userId);
            var customers = await _customerServices.CustomersBySellerIdAsync(userId);

            var model = new SellerViewModel
            {
                ItemsForSale = itemsForSale,
                Customers = customers
            };

            return View(model);
        }

        public IActionResult AddElectronics()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }
            return View(new Electronics
            {
                ElectronicId = 0,
                ElectronicName = string.Empty,
                ElectronicModel = string.Empty,
                Description = string.Empty,
                Image = string.Empty,
                Price = 0.0M,
                UserId = userId,
                Stock = 0
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddElectronics(Electronics product)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    return Unauthorized();
                }
                product.UserId = userId;
                await _electronicServices.AddItemAsync(product);
                return RedirectToAction("MyElectronics");
            }
            return View(product);
        }



        public async Task<IActionResult> EditElectronics(int id)
        {
            var product = await _electronicServices.ItemByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> EditElectronics(Electronics product)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    return Unauthorized();
                }

                // Ensure the product belongs to the current user
                var existingProduct = await _electronicServices.ItemByIdAsync(product.ElectronicId);
                if (existingProduct == null || existingProduct.UserId != userId)
                {
                    return Unauthorized();
                }

                // Update the product details
                existingProduct.ElectronicName = product.ElectronicName;
                existingProduct.ElectronicModel = product.ElectronicModel;
                existingProduct.Description = product.Description;
                existingProduct.Image = product.Image;
                existingProduct.Price = product.Price;
                existingProduct.Stock = product.Stock;

                await _electronicServices.UpdateItemAsync(existingProduct);
                return RedirectToAction("MyElectronics");
            }
            return View(product);
        }


        [HttpPost]
        public async Task<IActionResult> DeleteElectronic(int id)
        {
            var product = await _electronicServices.ItemByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (product.UserId != userId)
            {
                return Unauthorized();
            }

            await _electronicServices.DeleteItemAsync(id);
            return RedirectToAction("MyElectronics");
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
