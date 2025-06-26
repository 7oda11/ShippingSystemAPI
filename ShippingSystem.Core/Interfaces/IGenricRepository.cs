using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.Core.Interfaces
{
    
        public interface IGenricRepository<T> where T : class
        {
            public List<T> GetAll();
            //public T Get(int id);
            public T GetById(int id);
            public void Add(T entity);
            public void Update(T entity);

            public void Delete(T entity);
        }
    
}
