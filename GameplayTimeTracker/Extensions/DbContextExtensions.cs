using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GameplayTimeTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace GameplayTimeTracker.Extensions;

public static class DbContextExtensions
{
    /// <summary>
    /// Adds a new entity if it doesn't exist, or updates it if it does.
    /// The entity must have a primary key defined.
    /// </summary>
    public static async Task AddOrUpdateAsync<T>(
        this DbContext context,
        T entity,
        Expression<Func<T, object>> identifierExpression
    ) where T : BaseDataModel
    {
        var dbSet = context.Set<T>();

        // Extract property name safely
        string propertyName;

        if (identifierExpression.Body is MemberExpression member)
            propertyName = member.Member.Name;
        else if (identifierExpression.Body is UnaryExpression unary && unary.Operand is MemberExpression memberOperand)
            propertyName = memberOperand.Member.Name;
        else
            throw new InvalidOperationException("Identifier expression is not valid");

        // Get the identifier value
        var identifierValue = identifierExpression.Compile()(entity);

        // Look for existing entity
        var existing = await dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(e => EF.Property<object>(e, propertyName).Equals(identifierValue));

        if (existing == null)
        {
            await dbSet.AddAsync(entity);
        }
        else
        {
            context.Entry(entity).State = EntityState.Modified;
        }
    }
}