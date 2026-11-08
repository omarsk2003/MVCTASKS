using Microsoft.AspNetCore.Mvc;
using TASK6.DATA;
using TASK6.Models;

namespace TASK6.Controllers

{
    public class CategoryController : Controller
    {
        private readonly CafeteriaEmpDbContext _context;

        public CategoryController(CafeteriaEmpDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var categories = _context.Categories.ToList();
            return View(categories);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category category)
        {
         
                _context.Categories.Add(category);
                _context.SaveChanges();
                return RedirectToAction("Index");
            
        }

    }
}
