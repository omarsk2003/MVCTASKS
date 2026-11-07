using Microsoft.AspNetCore.Mvc;
using AcademyGraduationSupply.Models;
namespace AcademyGraduationSupply.Controllers
{
    public class SupplyController : Controller
    {
        private readonly AcademyGraduationPreparationDbContext _context;

        public SupplyController(AcademyGraduationPreparationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SelectOffers(string Category1,string Category2,string Category3)
        {
            if(string.IsNullOrEmpty(Category1) || string.IsNullOrEmpty(Category2) || string.IsNullOrEmpty(Category3))
            {
                TempData["Message"] = "Please select one offer from each category!";
                return View("Index");
            }

            //DB
            List<string> supplierNames = new List<string>();

            supplierNames.Add(Category1.Replace("Supplier", "Supplier "));
            supplierNames.Add(Category2.Replace("Supplier", "Supplier "));
            supplierNames.Add(Category3.Replace("Supplier", "Supplier "));

            List<int> offerIds = new List<int>();

            foreach (string supplierName in supplierNames)
            {
                int offerId = (
                    from offer in _context.Offers
                    join supplier in _context.Suppliers
                        on offer.SupplierId equals supplier.SupplierId
                    where supplier.SupplierName == supplierName
                    select offer.OfferId
                ).FirstOrDefault();

                if (offerId == 0)
                {
                    TempData["Message"] = "Selected offer was not found!";
                    return RedirectToAction("Index");
                }

                offerIds.Add(offerId);
            }

            using (var transaction = _context.Database.BeginTransaction())
            {
                SupplyRequest request = new SupplyRequest();

                _context.SupplyRequests.Add(request);
                _context.SaveChanges();

                foreach (int offerId in offerIds)
                {
                    SelectedOffer selected = new SelectedOffer();

                    selected.RequestId = request.RequestId;
                    selected.OfferId = offerId;

                    _context.SelectedOffers.Add(selected);
                }

                _context.SaveChanges();
                transaction.Commit();
            }



            //Session 
            HttpContext.Session.SetString("Category1", Category1);
            HttpContext.Session.SetString("Category2", Category2);
            HttpContext.Session.SetString("Category3", Category3);



            //List
            List<string> selectedOffers = new List<string>();

            selectedOffers.Add(Category1);
            selectedOffers.Add(Category2);
            selectedOffers.Add(Category3);

            string offers = string.Join("|", selectedOffers);
            HttpContext.Session.SetString("SelectedOffers", offers);
          
            
            return RedirectToAction("Review");
        }


        [HttpGet]
        public IActionResult Review()
        {

            string offers =
                HttpContext.Session.GetString("SelectedOffers") ?? "";

            List<string> selectedOffers = offers
                .Split('|', StringSplitOptions.RemoveEmptyEntries)
                .ToList();


            string certificate = HttpContext.Session.GetString("Category1") ?? "";
            string hospitality = HttpContext.Session.GetString("Category2") ?? "";
            string gift = HttpContext.Session.GetString("Category3") ?? "";

            if (string.IsNullOrEmpty(certificate) ||
                string.IsNullOrEmpty(hospitality) ||
                string.IsNullOrEmpty(gift))
            {
                TempData["Message"] = "Please select all three offers!";
                return RedirectToAction("Index");
            }


            string certificateOffer = "";
            int certificateQuantity = 35;
            decimal certificatePrice = 0;

            switch (certificate)
            {
                case "SupplierA":
                    certificateOffer = "Premium Certificate Package";
                    certificatePrice = 2m;
                    break;

                case "SupplierB":
                    certificateOffer = "Standard Certificate Package";
                    certificatePrice = 1.5m;
                    break;

                case "SupplierC":
                    certificateOffer = "Luxury Certificate Package";
                    certificatePrice = 2.5m;
                    break;

                default:
                    return RedirectToAction("Index");
            }

            decimal certificateTotal = certificateQuantity * certificatePrice;

            string hospitalityOffer = "";
            int hospitalityQuantity = 100;
            decimal hospitalityPrice = 0;

            switch (hospitality)
            {
                case "SupplierD":
                    hospitalityOffer = "Coffee & Drinks Package";
                    hospitalityPrice = 2m;
                    break;

                case "SupplierE":
                    hospitalityOffer = "Sandwiches & Snacks Package";
                    hospitalityPrice = 3m;
                    break;

                case "SupplierF":
                    hospitalityOffer = "Full Hospitality Package";
                    hospitalityPrice = 4m;
                    break;

                default:
                    return RedirectToAction("Index");
            }

            decimal hospitalityTotal = hospitalityQuantity * hospitalityPrice;

            string giftOffer = "";
            int giftQuantity = 35;
            decimal giftPrice = 0;

            switch (gift)
            {
                case "SupplierG":
                    giftOffer = "Customized Mugs Package";
                    giftPrice = 2m;
                    break;

                case "SupplierH":
                    giftOffer = "Graduation Bags Package";
                    giftPrice = 3m;
                    break;

                case "SupplierI":
                    giftOffer = "Premium Gift Boxes Package";
                    giftPrice = 4m;
                    break;

                default:
                    return RedirectToAction("Index");
            }

            decimal giftTotal = giftQuantity * giftPrice;


            decimal totalPrice = certificateTotal + hospitalityTotal + giftTotal;

            decimal discountPercentage = 0;

            if (totalPrice > 500)
            {
                discountPercentage = 0.15m;
            }
            else if (totalPrice > 300)
            {
                discountPercentage = 0.10m;
            }

            decimal discountAmount = totalPrice * discountPercentage;

            decimal finalTotal = totalPrice - discountAmount;

            ViewBag.CertificateSupplier = certificate;
            ViewBag.CertificateOffer = certificateOffer;
            ViewBag.CertificateQuantity = certificateQuantity;
            ViewBag.CertificatePrice = certificatePrice;
            ViewBag.CertificateTotal = certificateTotal;

            ViewBag.HospitalitySupplier = hospitality;
            ViewBag.HospitalityOffer = hospitalityOffer;
            ViewBag.HospitalityQuantity = hospitalityQuantity;
            ViewBag.HospitalityPrice = hospitalityPrice;
            ViewBag.HospitalityTotal = hospitalityTotal;

            ViewBag.GiftSupplier = gift;
            ViewBag.GiftOffer = giftOffer;
            ViewBag.GiftQuantity = giftQuantity;
            ViewBag.GiftPrice = giftPrice;
            ViewBag.GiftTotal = giftTotal;

            ViewBag.TotalPrice = totalPrice;
            ViewBag.DiscountPercentage = discountPercentage * 100;
            ViewBag.DiscountAmount = discountAmount;
            ViewBag.FinalTotal = finalTotal;

            return View();
        }
        public IActionResult About()
        {
            return View();
        }

    }
}
