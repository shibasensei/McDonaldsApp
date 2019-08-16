using Casestudy.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Casestudy.Models
{
    public class TrayModel
    {
        private AppDbContext _db;

        public TrayModel(AppDbContext ctx)
        {
            _db = ctx;
        }
        public int AddTray(Dictionary<string, object> items, ApplicationUser user)
        {
            int trayId = -1;
            using (_db)
            {
                // we need a transaction as multiple entities involved
                using (var _trans = _db.Database.BeginTransaction())
                {
                    try
                    {
                        Order tray = new Order();
                        tray.UserId = user.Id;
                        tray.OrderDate = System.DateTime.Now;
                        tray.OrderAmount = 0;
                        // calculate the totals and then add the tray row to the table
                        foreach (var key in items.Keys)
                        {
                            ProductViewModel item =
                           JsonConvert.DeserializeObject<ProductViewModel>(Convert.ToString(items[key]));
                            if (item.Qty > 0)
                            {
                                tray.OrderAmount += item.Qty;
                                tray.OrderTotal += item.Qty * item.CostPrice;
                                
                            }
                        }
                        _db.Orders.Add(tray);
                        _db.SaveChanges();
                        // then add each item to the trayitems table
                        foreach (var key in items.Keys)
                        {
                            ProductViewModel item =
                           JsonConvert.DeserializeObject<ProductViewModel>(Convert.ToString(items[key]));
                            if (item.Qty > 0)
                            {
                                OrderLineItem order = new OrderLineItem();
                                order.ProductId = item.Id;
                                order.OrderId = tray.Id;

                                Product product = _db.Products.First(p => p.Id == item.Id);

                                if (product.QtyOnHand >= item.Qty)
                                {
                                    
                                    order.QtySold = item.Qty;
                                    order.QtyOrdered = item.Qty;
                                    order.QtyBackOrdered = 0;
                                    product.QtyOnHand -= item.Qty;
                                    order.SellingPrice += item.CostPrice * order.QtySold;
                                }
                                else
                                {
                                    
                                    order.QtySold = product.QtyOnHand;
                                    order.QtyOrdered = item.Qty;
                                    order.QtyBackOrdered = item.Qty - product.QtyOnHand; ;
                                    product.QtyOnBackOreder += item.Qty - product.QtyOnHand;
                                    product.QtyOnHand = 0;
                                    order.SellingPrice += item.CostPrice * order.QtySold;
                                }
                                //if (item.Qty > 0)
                                //{
                                //    OrderLineItem tItem = new OrderLineItem();



                                //    tItem.QtyOrdered = item.Qty;
                                //    tItem.OrderId = tray.Id;
                                //    tItem.ProductId = item.Id;
                                //    tItem.QtySold = item.Qty;
                                //    tItem.SellingPrice = item.CostPrice;
                                    _db.OrderLineItems.Add(order);
                                    _db.SaveChanges();
                                }
                            }
                        // test trans by uncommenting out these 3 lines
                        //int x = 1;
                        //int y = 0;
                        //x = x / y; 
                        _trans.Commit();
                        trayId = tray.Id;
                    }
                    catch (Exception ex)
                    {
                        trayId = -1;
                        Console.WriteLine(ex.Message);
                        _trans.Rollback();
                    }
                }
            }
            return trayId;
        }
        public List<Order> GetAllTrays(ApplicationUser user)
        {
            return _db.Orders.Where(tray => tray.UserId == user.Id).ToList<Order>();
        }
        public List<TrayViewModel> GetTrayDetails(int tid, string uid)
        {
            List<TrayViewModel> allDetails = new List<TrayViewModel>();
            // LINQ way of doing INNER 
            var results = from t in _db.Set<Order>()
                          join ti in _db.Set<OrderLineItem>() on t.Id equals ti.OrderId
                          join mi in _db.Set<Product>() on ti.ProductId equals mi.Id
                          where (t.UserId == uid && t.Id == tid)
                          select new TrayViewModel
                          {
                              TrayId = mi.Id,
                              UserId = uid,
                              Description = mi.ProductName,
                              Price = mi.CostPrice,
                              TotalOne = mi.CostPrice * ti.QtyOrdered,
                              TotalAll = t.OrderTotal*1.13m,
                              TotalPrice = t.OrderTotal,
                              TotalTax = t.OrderTotal * 0.13m,
                              QtyS = ti.QtySold,
                              QtyO = ti.QtyOrdered,
                              QtyB = ti.QtyBackOrdered,
                              DateCreated = t.OrderDate.ToString("yyyy/MM/dd - hh:mm tt")
                          };
            allDetails = results.ToList<TrayViewModel>();
            return allDetails;
        }
    }
}