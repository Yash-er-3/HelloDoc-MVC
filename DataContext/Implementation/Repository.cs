﻿using HelloDoc;
using Microsoft.EntityFrameworkCore;
using Services.Contracts;
using System.Linq.Expressions;

namespace Services.Implementation
{
    public class Repository<T> : IRepository<T> where T : class
    {

        private readonly HelloDocDbContext _dbContext;
        internal DbSet<T> dbSet;

        public Repository(HelloDocDbContext dbContext)
        {
            _dbContext = dbContext;
            this.dbSet = _dbContext.Set<T>();
        }

        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public IEnumerable<T> GetAll()
        {
            IQueryable<T> query = dbSet;
            return query.ToList();
        }

        public T GetFirstOrDefault(Expression<Func<T, bool>> filter)
        {
            IQueryable<T> query = dbSet;
            query = query.Where(filter);
            return query.FirstOrDefault();
        }



    }
}
