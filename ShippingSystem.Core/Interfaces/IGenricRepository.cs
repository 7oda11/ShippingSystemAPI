using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.Core.Interfaces
{
    
        public interface IGenricRepository<T> where T : class
        {
             Task <List<T>> GetAll();
            //public T Get(int id);
              Task<T> GetById(int id);
             Task Add(T entity);
             Task Update(T entity);

             Task Delete(T entity);
        }
    
}
