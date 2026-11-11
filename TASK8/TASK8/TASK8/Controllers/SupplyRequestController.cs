using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TASK8.Data;
using TASK8.Models;

namespace TASK8.Controllers
{
    [Authorize]
    public class SupplyRequestController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public SupplyRequestController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private bool IsAdmin => User.IsInRole("Admin") || User.IsInRole("SuperAdmin");
        private string? CurrentUserId => _userManager.GetUserId(User);

        public IActionResult Index()
        {
            var requests = _context.SupplyRequests.Include(r => r.User).AsQueryable();
            if (!IsAdmin) requests = requests.Where(r => r.UserId == CurrentUserId);
            return View(requests.OrderByDescending(r => r.RequestDate).ToList());
        }

        public IActionResult Details(int id)
        {
            var request = _context.SupplyRequests.Include(r => r.User)
                .Include(r => r.RequestItems).ThenInclude(i => i.Product)
                .FirstOrDefault(r => r.Id == id);
            if (request == null) return NotFound();
            if (!IsAdmin && request.UserId != CurrentUserId) return Forbid();
            return View(request);
        }

        private void LoadProducts() => ViewBag.Products = _context.Products.OrderBy(p => p.Name).ToList();
        private RequestCreateDTO EmptyRequest() => new RequestCreateDTO
        {
            Items = _context.Products.OrderBy(p => p.Name)
                .Select(p => new RequestLineDTO { ProductId = p.Id })
                .ToList()
        };

        [HttpGet]
        public IActionResult Create()
        {
            LoadProducts();
            return View(EmptyRequest());
        }

        private List<(Product product, int quantity)>? CheckItems(RequestCreateDTO model)
        {
            var lines = model.Items.Where(i => i.ProductId > 0 && i.Quantity > 0).ToList();
            if (lines.Count == 0)
            {
                ModelState.AddModelError("", "Select at least one product and quantity.");
                return null;
            }
            if (lines.GroupBy(i => i.ProductId).Any(g => g.Count() > 1))
            {
                ModelState.AddModelError("", "Do not select the same product twice.");
                return null;
            }
            if (model.Items.Any(i => i.Quantity < 0 || (i.ProductId == 0 && i.Quantity != 0)))
            {
                ModelState.AddModelError("", "Quantities cannot be negative.");
                return null;
            }
            var result = new List<(Product product, int quantity)>();
            foreach (var line in lines)
            {
                var product = _context.Products.Find(line.ProductId);
                if (product == null)
                {
                    ModelState.AddModelError("", "A selected product was not found.");
                    return null;
                }
                result.Add((product, line.Quantity));
            }
            return result;
        }

        [HttpPost]
        public IActionResult Create(RequestCreateDTO model)
        {
            model.Items ??= new();
            var items = CheckItems(model);
            if (items == null)
            {
                LoadProducts();
                return View(model);
            }
            var request = new SupplyRequest
            {
                UserId = CurrentUserId!,
                RequestDate = DateTime.Now,
                Status = "Pending",
                TotalPrice = items.Sum(x => x.product.Price * x.quantity),
                RequestItems = items.Select(x => new RequestItem
                {
                    ProductId = x.product.Id,
                    Quantity = x.quantity,
                    Price = x.product.Price
                }).ToList()
            };
            _context.SupplyRequests.Add(request);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var request = _context.SupplyRequests.Include(r => r.RequestItems)
                .FirstOrDefault(r => r.Id == id);
            if (request == null) return NotFound();
            if (request.UserId != CurrentUserId) return Forbid();
            if (request.Status != "Pending") return BadRequest("Only pending requests can be edited.");
            var model = new RequestCreateDTO
            {
                Items = _context.Products.OrderBy(p => p.Name)
                    .Select(p => new RequestLineDTO
                    {
                        ProductId = p.Id,
                        Quantity = request.RequestItems
                            .Where(i => i.ProductId == p.Id)
                            .Select(i => i.Quantity)
                            .FirstOrDefault()
                    }).ToList()
            };
            LoadProducts();
            ViewBag.RequestId = id;
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(int id, RequestCreateDTO model)
        {
            var request = _context.SupplyRequests.Include(r => r.RequestItems)
                .FirstOrDefault(r => r.Id == id);
            if (request == null) return NotFound();
            if (request.UserId != CurrentUserId) return Forbid();
            if (request.Status != "Pending") return BadRequest("Only pending requests can be edited.");
            model.Items ??= new();
            var items = CheckItems(model);
            if (items == null)
            {
                LoadProducts();
                ViewBag.RequestId = id;
                return View(model);
            }
            _context.RequestItems.RemoveRange(request.RequestItems);
            request.RequestItems = items.Select(x => new RequestItem
            {
                ProductId = x.product.Id, Quantity = x.quantity, Price = x.product.Price
            }).ToList();
            request.TotalPrice = items.Sum(x => x.product.Price * x.quantity);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var request = _context.SupplyRequests.FirstOrDefault(r => r.Id == id);
            if (request == null) return NotFound();
            if (!IsAdmin && request.UserId != CurrentUserId) return Forbid();
            if (request.Status != "Pending") return BadRequest("Only pending requests can be deleted.");
            _context.SupplyRequests.Remove(request);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        public IActionResult Approve(int id)
        {
            // Approve the request by changing its status.
            var request = _context.SupplyRequests.Include(r => r.RequestItems)
                .FirstOrDefault(r => r.Id == id);
            if (request == null) return NotFound();
            if (request.Status != "Pending") return BadRequest("This request was already processed.");
            request.Status = "Approved";
            _context.SaveChanges();
            return RedirectToAction("Details", new { id });
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        public IActionResult Reject(int id)
        {
            var request = _context.SupplyRequests.Find(id);
            if (request == null) return NotFound();
            if (request.Status != "Pending") return BadRequest("This request was already processed.");
            request.Status = "Rejected";
            _context.SaveChanges();
            return RedirectToAction("Details", new { id });
        }
    }
}
