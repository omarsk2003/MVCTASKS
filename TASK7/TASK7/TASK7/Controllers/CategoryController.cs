using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TASK7.Data;
using TASK7.Models;

namespace TASK7.Controllers
{
    public class CategoryController : Controller
    {
        private readonly CafeteriaDbContext _context;

        public CategoryController(CafeteriaDbContext context)
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


        [HttpGet]
        public IActionResult Edit(int id)
        {
            return View(_context.Categories.Find(id));
        }

        [HttpPost]
        public IActionResult Edit(Category category)
        {
            _context.Update(category);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            return View(_context.Categories.Find(id));
        }

        public IActionResult Delete(int id)
        {
            var user = _context.Categories.Find(id);
            _context.Remove(user);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
