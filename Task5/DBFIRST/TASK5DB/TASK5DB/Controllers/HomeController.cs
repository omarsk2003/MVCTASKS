using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TASK5DB.Data;
using TASK5DB.Models;

namespace TASK5DB.Controllers
{
    public class HomeController : Controller
    {
        private readonly CafeteriaDbContext _context;

        public HomeController(CafeteriaDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var products = _context.Products.ToList();

            return View(products);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
