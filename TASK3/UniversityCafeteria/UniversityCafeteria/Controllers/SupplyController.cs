using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections;
using System.ComponentModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UniversityCafeteria.Controllers
{
    public class SupplyController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Categories = new string[] { "Drinks", "Food", "Snacks", "Hot Drinks", "Cold Drinks" };

            return View();
        }
        public IActionResult Products()
        {
         
      
            ViewData["PageTitle"]= "Cafeteria Products";
            ViewData["ManagerName"] = "Ahmad Ali";
            ViewData["NumberOfProducts"] = 5;
            return View();
        }
        public IActionResult Suppliers()
        {
         ViewBag.PageTitle = "Our Suppliers";
         ViewBag.NumberOfSuppliers = 3;
         ViewBag.SupplierNames = new string[] { "Fresh Food Company", "Happy Drinks Company", "Daily Supplies Company" };
            return View();
        }

        [HttpGet]
        public IActionResult CreateRequest()
        {
            return View();
        }


        [HttpPost]
        public IActionResult SubmitRequest()
        {
            string employeeName = Request.Form["EmployeeName"].ToString();
            string productName = Request.Form["ProductName"].ToString();
            string quantity = Request.Form["Quantity"].ToString();
            string requiredDate = Request.Form["RequiredDate"].ToString();

            if (string.IsNullOrWhiteSpace(employeeName))
            {
                ViewBag.Message = "Error: Employee Name is required!";
                return View("CreateRequest");
            }

            TempData["Message"] = "Request submitted successfully!";

            return RedirectToAction("About");
        }
        public IActionResult About()
        {
            return View();
        }
    }
}
