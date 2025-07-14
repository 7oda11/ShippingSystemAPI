using Microsoft.EntityFrameworkCore;
using ShippingSystem.Core.Entities;
using ShippingSystem.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.BL.Repositories
{
    public class StatusRepository: GenericRepository<Core.Entities.Status>, Core.Interfaces.IStatusRepository
    {
        private readonly ShippingContext context;

        public StatusRepository(ShippingContext context) : base(context)
        {
            this.context = context;
        }


        public async Task<IEnumerable<Status>> GetAllAsync()
        {
            return await context.Statuses.ToListAsync();
        }

        public async Task<Status> GetByIdAsync(int id)
        {
            return await context.Statuses.FindAsync(id);
        }

        public async Task<Status> AddAsync(Status status)
        {
            context.Statuses.Add(status);
            await context.SaveChangesAsync();
            return status;
        }

        public async Task UpdateAsync(Status status)
        {
            context.Entry(status).State = EntityState.Modified;
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Status status)
        {
            context.Statuses.Remove(status);
            await context.SaveChangesAsync();
        }
        public async Task<Status> FindByNameAsync(string name)
        {
            return await _context.Statuses
                .FirstOrDefaultAsync(s => s.Name == name);
        }

        public async Task<int> GetStatusIdByNameAsync(string name)
        {
            var status = await _context.Statuses
                .FirstOrDefaultAsync(s => s.Name == name);

            return status?.Id ?? 0;
        }

    }
}
