using Microsoft.AspNetCore.Mvc;
using Casestudy.Models;
using System.Collections.Generic;
using Casestudy.Utils;
using System;
namespace Casestudy.Controllers
{
    public class BrandController : Controller
    {
        AppDbContext _db;
        public BrandController(AppDbContext context)
        {
            _db = context;
        }
        public IActionResult Index(BrandViewModel vm)
        {
            // only build the catalogue once
            if (HttpContext.Session.Get<List<Brand>>("brands") == null)
            {
                // no session information so let's go to the database
                try
                {
                    BrandModel catModel = new BrandModel(_db);
                    // now load the categories
                    List<Brand> brands = catModel.GetAll();
                    HttpContext.Session.Set<List<Brand>>("brands", brands);
                    vm.SetBrands(brands);
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "Catalogue Problem - " + ex.Message;
                }
            }
            else
            {
                // no need to go back to the database as information is already in the session
                vm.SetBrands(HttpContext.Session.Get<List<Brand>>("brands"));
            }
            return View(vm);
        }
        public IActionResult SelectBrand(BrandViewModel vm)
        {
            BrandModel catModel = new BrandModel(_db);
            ProductModel menuModel = new ProductModel(_db);
            List<Product> items = menuModel.GetAllByBrand(vm.BrandId);
            List<ProductViewModel> vms = new List<ProductViewModel>();
            if (items.Count > 0)
            {
                foreach (Product item in items)
                {
                    ProductViewModel mvm = new ProductViewModel();
                    mvm.Qty = 0;
                    mvm.BrandId = item.BrandId;
                    mvm.BrandName = catModel.GetName(item.BrandId);
                    mvm.Description = item.Description;
                    mvm.Id = item.Id;
                    mvm.MSRP = item.MSRP;
                    mvm.QtyOnBackOreder = item.QtyOnBackOreder;
                    mvm.QtyOnHand = item.QtyOnHand;
                    mvm.CostPrice = item.CostPrice;
                    mvm.GraphicName = item.GraphicName;
                    mvm.ProductName = item.ProductName;
                    vms.Add(mvm);
                }
                ProductViewModel[] myMenu = vms.ToArray();
                HttpContext.Session.Set<ProductViewModel[]>("menu", myMenu);
            }
            vm.SetBrands(HttpContext.Session.Get<List<Brand>>("brands"));
            return View("Index", vm); // need the original Index View here
        }        public ActionResult SelectProduct(BrandViewModel vm)
        {
            Dictionary<int, object> tray;
            if (HttpContext.Session.Get<Dictionary<int, Object>>("tray") == null)
            {
                tray = new Dictionary<int, object>();
            }
            else
            {
                tray = HttpContext.Session.Get<Dictionary<int, object>>("tray");
            }
            ProductViewModel[] menu = HttpContext.Session.Get<ProductViewModel[]>("menu");
            String retMsg = "";
            foreach (ProductViewModel item in menu)
            {
                if (item.Id == vm.Id)
                {
                    if (vm.Qty > 0) // update only selected item
                    {
                        item.Qty = vm.Qty;
                        retMsg = vm.Qty + " - item(s) Added!";
                        tray[item.Id] = item;
                    }
                    else
                    {
                        item.Qty = 0;
                        tray.Remove(item.Id);
                        retMsg = "item(s) Removed!";
                    }
                    vm.BrandId = item.BrandId;
                    break;
                }
            }
            ViewBag.AddMessage = retMsg;
            HttpContext.Session.Set<Dictionary<int, Object>>("tray", tray);
            vm.SetBrands(HttpContext.Session.Get<List<Brand>>("brands"));
            return View("Index", vm);
        }
    }


}
