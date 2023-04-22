using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Casestudy.DAL.DomainClasses;
namespace Casestudy.DAL
{
    public class DataUtility
    {
        private AppDbContext _db;

        public DataUtility(AppDbContext context) {
            _db = context;
        }

        public async Task<bool> loadProductInfoFromWebToDb(string stringJson)
        {
            bool brandsLoaded = false;
            bool productsLoaded = false;
            try
            { // an element that is typed as dynamic is assumed to support any operation
                dynamic objectJson = JsonSerializer.Deserialize<Object>(stringJson);
                brandsLoaded = await loadBrands(objectJson);
                productsLoaded = await loadProducts(objectJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return brandsLoaded && productsLoaded;
        }

        private async Task<bool> loadBrands(dynamic jsonObjectArray)
        {
            bool loadedBrands = false;
            try
            {
                // clear out the old rows
                _db.Brands.RemoveRange(_db.Brands);
                await _db.SaveChangesAsync();
                List<String> allBrands = new List<String>();
                foreach (JsonElement element in jsonObjectArray.EnumerateArray())
                {
                    if (element.TryGetProperty("BRAND", out JsonElement productJson))
                    {
                        allBrands.Add(productJson.GetString());
                    }
                }
                IEnumerable<String> brands = allBrands.Distinct<String>(); 
                foreach (string brandName in brands)
                {
                    Brand brnd = new Brand();
                    brnd.Name = brandName;
                    await _db.Brands.AddAsync(brnd);
                    await _db.SaveChangesAsync();
                }
                loadedBrands = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error - " + ex.Message);
            }
            return loadedBrands;
        }
        private async Task<bool> loadProducts(dynamic jsonObjectArray)
        {
            bool loadedItems = false;
            try
            {
                List<Brand> categories = _db.Brands.ToList();
                // clear out the old
                _db.Products.RemoveRange(_db.Products);
                await _db.SaveChangesAsync();
                foreach (JsonElement element in jsonObjectArray.EnumerateArray())
                {
                    Product item = new Product();
                    item.Id = element.GetProperty("ID").GetString();
                    item.ProductName = element.GetProperty("PRODUCTNAME").GetString();
                    item.GraphicName = element.GetProperty("GRAPHICNAME").GetString();
                    item.CostPrice = Convert.ToSingle(element.GetProperty("COSTPRICE").ToString());
                    item.MSRP= Convert.ToDecimal(element.GetProperty("MSRP").ToString());
                    item.QtyOnHand = Convert.ToInt32(element.GetProperty("QTYONHAND").ToString());
                    item.QtyOnBackOrder = Convert.ToInt32(element.GetProperty("QTYONBACKORDER").ToString());

                    
                    //item.Description = element.GetProperty("DESCRIPTION").GetString();

                    string brnd = element.GetProperty("BRAND").GetString();
                    // add the FK here
                    foreach (Brand category in categories)
                    {
                        if (category.Name == brnd)
                        {
                            item.Brand = category;
                            break;
                        }
                    }
                    await _db.Products.AddAsync(item);
                    await _db.SaveChangesAsync();
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
