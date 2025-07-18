﻿using ShippingSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.Core.Interfaces
{
    public interface IOrderCancellationRepository : IGenericRepository<OrderCancellation> 
    {
        Task<List<OrderCancellation>> GetReasonsByOrderIds(List<int> orderIds);

    }

}
