using ForgeAir.Core.Services.Database.Interfaces;
using ForgeAir.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML.SearchSpace;
using System;
using System.Linq.Expressions;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly IDbContextFactory<ForgeAirDbContext> _factory;

    public Repository(IDbContextFactory<ForgeAirDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        using var ctx = _factory.CreateDbContext();
        return await ctx.Set<T>().FindAsync(id);
    }

    public async Task<List<T>> GetAllAsync(
        Func<IQueryable<T>, IQueryable<T>> include = null)
    {
        using var ctx = _factory.CreateDbContext();

        IQueryable<T> query = ctx.Set<T>();

        if (include != null)
            query = include(query);

        return await query.ToListAsync();
    }
    public async Task<List<T>> GetAllAsync(params Expression<Func<T, object>>[] includes) { using var ctx = _factory.CreateDbContext(); IQueryable<T> query = ctx.Set<T>(); foreach (var include in includes) query = query.Include(include); return await query.ToListAsync(); }
    public async Task<List<T>> GetAllAsync() { using var ctx = _factory.CreateDbContext(); return await ctx.Set<T>().AsNoTracking().ToListAsync(); }

    public async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        using var ctx = _factory.CreateDbContext();
        return await ctx.Set<T>().Where(predicate).ToListAsync();
    }
    public async Task<bool> Exists(Expression<Func<T, bool>> predicate)
    {
        using var ctx = _factory.CreateDbContext();
        return await ctx.Set<T>().AnyAsync(predicate);
    }

    public async Task AddAsync(T entity)
    {
        using var ctx = _factory.CreateDbContext();
        ctx.Set<T>().Add(entity);
        await ctx.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        using var ctx = _factory.CreateDbContext();
        ctx.Set<T>().Update(entity);
        await ctx.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        using var ctx = _factory.CreateDbContext();
        ctx.Set<T>().Remove(entity);
        await ctx.SaveChangesAsync();
    }
}
