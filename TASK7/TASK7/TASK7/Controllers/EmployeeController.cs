using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TASK7.Data;
using TASK7.Models;

namespace TASK7.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly CafeteriaDbContext _context;

        public EmployeeController(CafeteriaDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View(_context.Employees.Include(e => e.Orders).ToList());
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Employee employee)
        {
            _context.Employees.Add(employee);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult Edit(int id)
        {
            return View(_context.Employees.Find(id));
        }

        [HttpPost]
        public IActionResult Edit(Employee employee)
        {
            _context.Employees.Update(employee);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            return View(_context.Employees.Find(id));
        }

        public IActionResult Delete(Employee employee)
        {
            _context.Employees.Remove(employee);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
