using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TASK7.Data;
using TASK7.Models;
namespace TASK7.Controllers
{
    public class OrderController : Controller
    {
        private readonly CafeteriaDbContext _context;

        public OrderController(CafeteriaDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View(_context.Orders
                .Include(p => p.Employee)
                .ToList());
        }
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Employees = _context.Employees.ToList();

            return View();
        }

        [HttpPost]
        public IActionResult Create(Order order)
        {
            _context.Orders.Add(order);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult Edit(int id)
        {
            ViewBag.Employees = _context.Employees.ToList();
            return View(_context.Orders.Find(id));
        }

        [HttpPost]
        public IActionResult Edit(Order order)
        {
            _context.Orders.Update(order);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            return View(_context.Orders
                .Include(p => p.Employee)
                .FirstOrDefault(p => p.Id == id));
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            _context.Orders.Remove(_context.Orders.Find(id));
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
