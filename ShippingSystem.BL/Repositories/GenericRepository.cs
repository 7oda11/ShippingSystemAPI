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
    public class GenericRepository<T> : IGenricRepository<T> where T : class
    {
        public ShippingContext context { get; }
        private readonly DbSet<T> _dbSet;

        public GenericRepository(ShippingContext ShippingContext)
        {
            this.context = ShippingContext;
            _dbSet = context.Set<T>();

        }


        public async Task Add(T entity)
        {
             await _dbSet.AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public async Task Delete(T entity)
        {
            _dbSet.Remove(entity);
            await context.SaveChangesAsync();
        }

        public async Task<List<T>> GetAll()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T> GetById(int id)
        {
            return await _dbSet.FindAsync(id);
        }



        public async Task Update(T entity)
        {
            context.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

            await context.SaveChangesAsync();

        }
    }
}
