using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShippingSystem.Core.DTO.Vendor;
using ShippingSystem.Core.Entities;
using ShippingSystem.Core.Interfaces;

﻿using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.BL.Repositories
{
    public class VendorRepository: GenericRepository<Vendor>, IVendorRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public readonly ShippingContext _context;

        public VendorRepository(ShippingContext context, UserManager<ApplicationUser> userMAnager) : base(context)
        {
           _userManager = userMAnager;
            this._context = context;
           
        }


       public async  Task<bool> AddNewVendor(AddVendorDTO vdto)
        {
            if(vdto == null)
            { return false; }

            var user = new ApplicationUser
            {
                FullName = vdto.name,
                UserName = vdto.email,
                Email = vdto.email,
                PhoneNumber = vdto.phone,

            };
            var result=  await _userManager.CreateAsync(user, vdto.password);
            if (!result.Succeeded) { return false; }

             var newVendor = new  Vendor{
                 Name = vdto.name,
                 Email = vdto.email,
                 Address = vdto.address,
                 UserId = user.Id,
                 GovernmentId = vdto.GovernmentId,
                 CityId = vdto.CityId,
                 Phones = new List<VendorPhones>
                 {
                     new VendorPhones {Phone = vdto.phone}
                 }
                 
            };
            await _context.Vendors.AddAsync(newVendor);
            await _context.SaveChangesAsync();
            return true;
        }

       
        public async Task<Vendor> FindByUserIdAsync(string userId)
        {
            return await _context.Vendors
                .Include(v => v.User)
                .FirstOrDefaultAsync(v => v.UserId == userId);
        }

        public async Task<bool> IsCityUsed(int cityId)
        {
            return await _context.Vendors.AnyAsync(v=>v.CityId == cityId);
        }
    }
}
