using System;
using System.Collections.Generic;
using System.Linq;
using dotnet_notely.Contracts;
using dotnet_notely.Data;
using Microsoft.EntityFrameworkCore;

namespace dotnet_notely.Repository;

public class GenericRepository<T> : IGenericManager<T> where T : class
{
    private readonly NotelyDbContext _context;
    
    public GenericRepository(NotelyDbContext notelyDbContext)
    {
        this._context = notelyDbContext;
    }
    
    public async Task<T> AddAsync(T entity)
    {
        await _context.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await GetAsync(id);
        _context.Set<T>().Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> Exists(int id)
    {
        var entity = await GetAsync(id);
        return entity != null;
    }

    public async Task<List<T>> GetAllAsync()
    {
        return await _context.Set<T>().ToListAsync();
    }

    public async Task<T> GetAsync(int? id)
    {
        if (id == null)
        {
            return null;
        }
        return await _context.Set<T>().FindAsync(id);
    }

    public async Task UpdateAsync(T entity)
    {
        _context.Update(entity);
        await _context.SaveChangesAsync();
    }
}