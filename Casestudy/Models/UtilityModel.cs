using Casestudy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Casestudy.Models
{
    public class UtilityModel
    {
        private AppDbContext _db;
        public UtilityModel(AppDbContext context) // will be sent by controller
        {
            _db = context;
        }
        public bool loadData(string stringJson)
        {
            bool brandsLoaded = false;
            bool productsLoaded = false;
            try
            {
                dynamic objectJson = Newtonsoft.Json.JsonConvert.DeserializeObject(stringJson);
                brandsLoaded = loadBrands(objectJson);
                productsLoaded = loadProducts(objectJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return brandsLoaded && productsLoaded;
        }
        private bool loadBrands(dynamic objectJson)
        {
            bool loadedCategories = false;
            try
            {
                // clear out the old rows
                _db.Brands.RemoveRange(_db.Brands);
                _db.SaveChanges();
                List<String> allBrands = new List<String>();
                foreach (var node in objectJson)
                {
                    allBrands.Add(Convert.ToString(node["BRAND"]));
                }
                // distinct will remove duplicates before we insert them into the db
                IEnumerable<String> categories = allBrands.Distinct<String>();
                foreach (string catname in categories)
                {
                    Brand cat = new Brand();
                    cat.Name = catname;
                    _db.Brands.Add(cat);
                    _db.SaveChanges();
                }
                loadedCategories = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - " + ex.Message);
            }
            return loadedCategories;
        }

        private bool loadProducts(dynamic objectJson)
        {
            bool loadedItems = false;
            try
            {
                List<Brand> brands = _db.Brands.ToList();
                // clear out the old
                _db.Products.RemoveRange(_db.Products);
                _db.SaveChanges();
                foreach (var node in objectJson)
                {
                    Product item = new Product();
                    item.ProductName = node["ProductName"].Value;
                    item.GraphicName = node["GraphicName"].Value;
                    item.CostPrice = Convert.ToDecimal(node["CostPrice"].Value);
                    item.MSRP = Convert.ToDecimal(node["MSRP"].Value);
                    item.QtyOnHand = Convert.ToInt32(node["QtyOnHand"].Value);
                    item.QtyOnBackOreder = Convert.ToInt32(node["QtyOnBackOrder"].Value);

                    item.Description = Convert.ToString(node["Description"]);
                    string cat = Convert.ToString(node["BRAND"].Value);
                    // add the FK here
                    foreach (Brand brand in brands)
                    {
                        if (brand.Name == cat)
                            item.Brand = brand;
                    }
                    _db.Products.Add(item);
                    _db.SaveChanges();
                }
                loadedItems = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - " + ex.Message);
            }
            return loadedItems;
        }
    }
}
