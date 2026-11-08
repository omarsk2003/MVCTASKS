using Microsoft.AspNetCore.Mvc;
using TASK6.DATA;
using TASK6.Models;

namespace TASK6.Controllers
{
    public class SupplierController : Controller
    {
        private readonly CafeteriaEmpDbContext _context;

        public SupplierController(CafeteriaEmpDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var suppliers = _context.Suppliers.ToList();
            return View(suppliers);
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
    }
}
