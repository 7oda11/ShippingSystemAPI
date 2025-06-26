using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ShippingSystem.Core.DTO;
using ShippingSystem.Core.Entities;
using ShippingSystem.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.BL.Repositories
{
    public class BranchRepository : GenericRepository<Core.Entities.Branch>, IBranchRepository
    {
        private readonly IMapper mapper;
        private readonly ShippingContext context;

        public BranchRepository(ShippingContext context, IMapper mapper) : base(context)
        {
            this.mapper = mapper;
            this.context = context;
        }

        //public async Task<List<Branch>> GetAllBranchesWithEmployesssAsync()
        //{
        //    return await context.Branches.Include(b => b.Employees).ToListAsync();
        //}


        //public async Task<Branch> GetBranchWithEmployeesAsync(int branchId)
        //{
        //    return await context.Branches
        //        .Include(b => b.Employees)
        //        .FirstOrDefaultAsync(b => b.Id == branchId);
        //}
        ////
        //public async Task<Branch> AddBranchAsync(Branch branch)
        //{
        //    var branch = context.Branches.Ad;
        //    await Add(branch);
        //    await context.SaveChangesAsync();
        //    return branch;
        //}
    }
}
