﻿using ShippingSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.Core.Interfaces
{
    public interface IProductRepository: IGenericRepository<Product>
    {
        Task<List<Product>>GetProductsByOrderId(int orderId);
    }
}
