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

    public bool ExistsById(int id) => Db.Set<T>().Find(id) != null;

    public virtual T? GetById(int id) => Db.Set<T>().Find(id);

    public void DeleteById(int id)
    {
        T? entity = Db.Set<T>().Find(id);
        if (entity != null) Db.Set<T>().Remove(entity);
    }

    public virtual void Add(T entity)
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

    public void Delete(T? entity)
    {
        if (entity != null)
        {
            Db.Set<T>().Remove(entity);
            Db.SaveChanges();
        }
        else
        {
            Console.WriteLine($"{typeof(T)} not found!");
        }
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