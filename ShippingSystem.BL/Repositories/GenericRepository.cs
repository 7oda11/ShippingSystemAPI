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


        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public List<T> GetAll()
        {
            return _dbSet.ToList();
        }

        public T GetById(int id)
        {
            return _dbSet.Find(id);
        }



        public void Update(T entity)
        {
            context.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

        }
    }
}
