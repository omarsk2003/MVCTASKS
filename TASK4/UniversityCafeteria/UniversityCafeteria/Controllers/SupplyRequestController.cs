using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace UniversityCafeteria.Controllers
{
    public class SupplyRequestController : Controller
    {
        static List<string> names = new List<string>();
        static List<string> categories = new List<string>();
        static List<int> quantities = new List<int>();
        static List<int> unitPrices = new List<int>();
        static List<string> priorities = new List<string>();

        [HttpGet]
        public IActionResult Index()
        {
            string names = HttpContext.Session.GetString("Names") ?? "";

            List<string> products = names
                .Split('|', StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            ViewBag.Products = products;

            return View();
        }


        [HttpPost]
        public IActionResult AddItem(string name, string category,
                                     int quantity, decimal unitPrice,
                                     string priority)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                TempData["Message"] = "Product Name is required!";
                return RedirectToAction("Index");
            }

            if (quantity <= 0 || unitPrice <= 0)
            {
                TempData["Message"] =
                    "Quantity and Unit Price must be greater than zero!";
                return RedirectToAction("Index");
            }

            string names = HttpContext.Session.GetString("Names") ?? "";
            string categories = HttpContext.Session.GetString("Categories") ?? "";
            string quantities = HttpContext.Session.GetString("Quantities") ?? "";
            string prices = HttpContext.Session.GetString("Prices") ?? "";

            names += name + "|";
            categories += category + "|";
            quantities += quantity + "|";
            prices += unitPrice.ToString(
                System.Globalization.CultureInfo.InvariantCulture) + "|";

            HttpContext.Session.SetString("Names", names);
            HttpContext.Session.SetString("Categories", categories);
            HttpContext.Session.SetString("Quantities", quantities);
            HttpContext.Session.SetString("Prices", prices);
            HttpContext.Session.SetString("Priority", priority);

            TempData["Message"] = "Product added successfully!";

            return RedirectToAction("Index");
        }




        [HttpGet]
        public IActionResult Review()
        {
            string names = HttpContext.Session.GetString("Names") ?? "";
            string quantities = HttpContext.Session.GetString("Quantities") ?? "";
            string prices = HttpContext.Session.GetString("Prices") ?? "";

            List<string> productNames = names
                .Split('|', StringSplitOptions.RemoveEmptyEntries).ToList();

            List<string> productQuantities = quantities
                .Split('|', StringSplitOptions.RemoveEmptyEntries).ToList();

            List<string> productPrices = prices
                .Split('|', StringSplitOptions.RemoveEmptyEntries).ToList();

            List<decimal> productTotals = new List<decimal>();

            decimal subtotal = 0;

            for (int i = 0; i < productNames.Count; i++)
            {
                int quantity = int.Parse(productQuantities[i]);

                decimal price = decimal.Parse(
                    productPrices[i],
                    System.Globalization.CultureInfo.InvariantCulture);

                decimal total = quantity * price;

                productTotals.Add(total);
                subtotal += total;
            }

            string priority =
                HttpContext.Session.GetString("Priority") ?? "Normal";

            decimal preparationFee = 0;

            switch (priority)
            {
                case "Normal":
                    preparationFee = 2;
                    break;

                case "Urgent":
                    preparationFee = 5;
                    break;

                case "Emergency":
                    preparationFee = 10;
                    break;
            }

            decimal discount = 0;

            if (subtotal > 200)
            {
                discount = subtotal * 0.15m;
            }
            else if (subtotal > 100)
            {
                discount = subtotal * 0.10m;
            }

            decimal finalTotal = subtotal + preparationFee - discount;

            ViewBag.Names = productNames;
            ViewBag.Quantities = productQuantities;
            ViewBag.UnitPrices = productPrices;
            ViewBag.ProductTotals = productTotals;

            ViewBag.Priority = priority;
            ViewBag.Subtotal = subtotal;
            ViewBag.PreparationFee = preparationFee;
            ViewBag.Discount = discount;
            ViewBag.FinalTotal = finalTotal;

            return View();
        }

        public IActionResult ConfirmRequest()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ClearDraft()
        {
            HttpContext.Session.Remove("Names");
            HttpContext.Session.Remove("Categories");
            HttpContext.Session.Remove("Quantities");
            HttpContext.Session.Remove("Prices");
            HttpContext.Session.Remove("Priority");

            TempData["Message"] = "Draft cleared successfully!";

            return RedirectToAction("Index");
        }
        public IActionResult SavePreference()
        {
            return View();
        }
        public IActionResult MyRequests()
        {
            return View();
        }
        public IActionResult About()
        {
            return View();
        }
    }
}
