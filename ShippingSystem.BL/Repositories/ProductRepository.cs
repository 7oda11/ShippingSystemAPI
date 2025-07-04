using Microsoft.EntityFrameworkCore;
using ShippingSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.BL.Repositories
{
    public class ProductRepository: GenericRepository<Core.Entities.Product>, Core.Interfaces.IProductRepository
    {
        public ProductRepository(ShippingContext context) : base(context)
        { }

        public async Task<List<Product>> GetProductsByOrderId(int orderId)
        {
            return await  _context.Products.Where(p=>p.OrderId == orderId).ToListAsync();
        }
    }
}
