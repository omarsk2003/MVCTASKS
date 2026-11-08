using Microsoft.AspNetCore.Mvc;
using TASK6.DATA;
using TASK6.Models;
namespace TASK6.Controllers
{
    public class ProductController : Controller
    {
       private readonly CafeteriaEmpDbContext _context;

        public ProductController(CafeteriaEmpDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var products = _context.Products.ToList();
            return View(products);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
