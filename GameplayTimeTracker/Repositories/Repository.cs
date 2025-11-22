using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using GameplayTimeTracker.Data;

namespace GameplayTimeTracker.Repositories;

public class Repository<T> where T : class
{
    protected readonly AppDbContext _db;

    public Repository(AppDbContext db)
    {
        _db = db;
    }

    public void Add(T entity)
    {
        _db.Set<T>().Add(entity);
        _db.SaveChanges();
    }

    public void Update(T entity)
    {
        _db.Set<T>().Update(entity);
        _db.SaveChanges();
    }

    public void Delete(T entity)
    {
        _db.Set<T>().Remove(entity);
        _db.SaveChanges();
    }

    public T? Get(Expression<Func<T, bool>> predicate)
    {
        return _db.Set<T>().FirstOrDefault(predicate);
    }

    public List<T> GetAll()
    {
        return _db.Set<T>().ToList();
    }
}