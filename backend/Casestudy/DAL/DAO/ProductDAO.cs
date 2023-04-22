using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Casestudy.DAL.DomainClasses;
using Microsoft.EntityFrameworkCore;


namespace Casestudy.DAL.DAO
{
    public class ProductDAO
    {
        private AppDbContext _db;
        public ProductDAO(AppDbContext ctx)
        {
            _db = ctx;
        }
        public async Task<List<Product>> GetAllByCategory(int id)
        {
            return await _db.Products.Where(item => item.Brand.Id == id).ToListAsync();
        }

        public async Task<Product> GetProduct(string id)
        {
            return await _db.Products.FirstOrDefaultAsync(item => item.Id == id);
        }

    }
}
