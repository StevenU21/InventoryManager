using Microsoft.EntityFrameworkCore;

namespace InventoryManager.Services
{
    public class BaseService<T> where T : class
    {
        protected readonly Data.AppDbContext _context;

        public BaseService(Data.AppDbContext context)
        {
            _context = context;
        }

        public virtual async Task<List<T>> GetAllAsync()
            => await _context.Set<T>().ToListAsync();

        public virtual async Task<T?> GetByIdAsync(int id)
            => await _context.Set<T>().FindAsync(id);

        public virtual async Task AddAsync(T entity)
        {
            _context.Set<T>().Add(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task UpdateAsync(T entity)
        {
            var entry = _context.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                var pkey = _context.Model.FindEntityType(typeof(T))?.FindPrimaryKey();
                if (pkey != null)
                {
                    var keyValues = pkey.Properties.Select(p => typeof(T).GetProperty(p.Name)?.GetValue(entity)).ToArray();
                    var tracked = _context.Set<T>().Local.FirstOrDefault(e =>
                        pkey.Properties.Select(p => typeof(T).GetProperty(p.Name)?.GetValue(e))
                        .SequenceEqual(keyValues)
                    );
                    if (tracked != null)
                    {
                        _context.Entry(tracked).State = EntityState.Detached;
                    }
                }
            }
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(int id)
        {
            var entity = await _context.Set<T>().FindAsync(id);
            if (entity != null)
            {
                _context.Set<T>().Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}