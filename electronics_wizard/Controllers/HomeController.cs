using electronics_wizard.Areas.Identity.Data;
using electronics_wizard.Data;
using electronics_wizard.Models;
using electronics_wizard.Services;
using electronics_wizard.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace electronics_wizard.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        private readonly UserManager<AppUserUser> _userManager;
        private readonly ElectronicServices _electronicServices;
        private readonly CustomerServices _customerServices;

        public HomeController(ILogger<HomeController> logger, AppDbContext context, UserManager<AppUserUser> userManager, ElectronicServices electronicServices, CustomerServices customerServices)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _electronicServices = electronicServices;
            _customerServices = customerServices;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var products = await _context.Electronics
                .Where(p => p.UserId != userId)
                .ToListAsync();
            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var products = string.IsNullOrEmpty(query)
                ? await _context.Electronics
                    .Where(p => p.UserId != userId)
                    .ToListAsync()
                : await _context.Electronics
                    .Where(p => p.ElectronicName.Contains(query) && p.UserId != userId)
                    .ToListAsync();

            return PartialView("Partials/_ProductsList", products);
        }

        public async Task<IActionResult> ElectronicDetails(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var product = await _context.Electronics.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var otherProducts = await _context.Electronics
                .Where(p => p.UserId != userId && p.ElectronicId != id)
                .Take(4)
                .ToListAsync();

            var viewModel = new ElectronicDetailsViewModel
            {
                Electronics = product,
                OtherElectronics = otherProducts
            };

            return View(viewModel);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
