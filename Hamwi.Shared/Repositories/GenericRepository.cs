using Microsoft.EntityFrameworkCore;
using Hamwi.Shared.Entities.Base;
using Hamwi.Shared.Filters.Entity.Interfaces;
using Hamwi.Shared.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hamwi.Shared.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : EntityBase, new()
    {
        private readonly HamwiContext _context;
        private readonly DbSet<T> _table;

        public GenericRepository(HamwiContext context)
        {
            _context = context;
            _table = _context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAsync()
            => await _table.ToListAsync();

        public async Task<IEnumerable<T>> GetAsync(IFilter<T> filter)
            => await filter.Build(_table).ToListAsync();

        public async Task<T> GetAsync(Guid id)
            => await _table.FindAsync(id);

        public async Task<decimal> CountAsync(IFilter<T> filter = null)
            => await ((filter != null) ? filter.Build(_table, false) : _table).CountAsync();

        public async Task<T> AddAsync(T entity)
        {
            var added = _context.Entry(entity);
            added.State = EntityState.Added;
            await _context.SaveChangesAsync();
            return added.Entity;
        }

        public async Task<T> UpdateAsync(T entity)
        {
            var updated = _context.Entry(entity);
            updated.State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return updated.Entity;
        }

        public async Task<T> DeleteAsync(Guid id)
        {
            var deleted = _context.Entry(await GetAsync(id));
            deleted.State = EntityState.Deleted;
            await _context.SaveChangesAsync();
            return deleted.Entity;
        }

        public async Task<bool> ExistsAsync(Guid id)
            => await _table.AnyAsync(entity => entity.Id.Equals(id));
    }
}