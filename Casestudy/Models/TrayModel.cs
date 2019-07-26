using Casestudy.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

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
                                OrderLineItem tItem = new OrderLineItem();
                                tItem.QtyOrdered = item.Qty;
                                tItem.OrderId = tray.Id;
                                tItem.ProductId = item.Id;
                                tItem.QtySold = item.Qty;
                                tItem.SellingPrice = item.CostPrice;
                                _db.OrderLineItems.Add(tItem);
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
    }
}