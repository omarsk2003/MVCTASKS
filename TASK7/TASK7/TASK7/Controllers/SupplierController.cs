using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TASK7.Data;
using TASK7.Models;

namespace TASK7.Controllers
{
    public class SupplierController : Controller
    {
        private readonly CafeteriaDbContext _context;

        public SupplierController(CafeteriaDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {

            var Suppliers = _context.Suppliers
            .Include(s => s.Products)
            .ToList();

            return View(Suppliers);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Supplier supplier)
        {
            _context.Suppliers.Add(supplier);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult Edit(int id)
        {
            return View(_context.Suppliers.Find(id));
        }

        [HttpPost]
        public IActionResult Edit(Supplier supplier)
        {
            _context.Suppliers.Update(supplier);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            return View(_context.Suppliers.Find(id));
        }

        public IActionResult Delete(Supplier supplier)
        {
            _context.Suppliers.Remove(supplier);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
