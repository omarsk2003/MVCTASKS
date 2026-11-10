using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TASK7.Data;
using TASK7.Models;

namespace TASK7.Controllers
{
    public class ProductController : Controller
    {
        private readonly CafeteriaDbContext _context;

        public ProductController(CafeteriaDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var products = _context.Products.Include(p => p.Category).Include(s => s.Supplier).ToList();
            return View(products);

        }
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Suppliers = _context.Suppliers.ToList();

            return View();
        }
        [HttpPost]
        public IActionResult Create(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult Edit(int id)
        {
            return View(_context.Products.Find(id));
        }

        [HttpPost]
        public IActionResult Edit(Product product)
        {
            _context.Products.Update(product);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            return View(_context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .FirstOrDefault(p => p.Id == id));
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            _context.Products.Remove(_context.Products.Find(id));
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
