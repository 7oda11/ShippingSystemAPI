﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.Core.Interfaces
{

    
        public interface IGenericRepository<T> where T : class
        {

           Task<List<T>> GetAll();
            //public T Get(int id);
             Task <T> GetById(int id);
            Task Add(T entity);
            Task Update(T entity);

            Task Delete(T entity);
        Task<bool> AnyAsync(Func<T, bool> predicate);


    }

}
