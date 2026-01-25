using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using GameplayTimeTracker.Data;

namespace GameplayTimeTracker.Repositories;

public class Repository<T>(AppDbContext db)
    where T : class
{
    protected readonly AppDbContext Db = db;

    public void Add(T entity)
    {
        Db.Set<T>().Add(entity);
        Console.WriteLine($"{typeof(T)} added successfully!");
        Db.SaveChanges();
    }

    public void Update(T entity)
    {
        Db.Set<T>().Update(entity);
        Db.SaveChanges();
    }

    public void Delete(T entity)
    {
        Db.Set<T>().Remove(entity);
        Db.SaveChanges();
    }

    public T? Get(Expression<Func<T, bool>> predicate)
    {
        return Db.Set<T>().FirstOrDefault(predicate);
    }

    public List<T> GetAll()
    {
        return Db.Set<T>().ToList();
    }
}