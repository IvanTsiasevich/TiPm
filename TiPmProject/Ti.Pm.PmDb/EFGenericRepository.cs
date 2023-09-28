using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Ti.Pm.PmDb.Model;

namespace Ti.Pm.PmDb
{
    public class EFRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private DbContext _context;
        private DbSet<TEntity> _dbSet;
        private IChangeLog IChangeLog ;

        public EFRepository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }
        
        public IEnumerable<TEntity> Get()
        {
            return _dbSet.AsNoTracking().ToList();
        }

        public IQueryable<TEntity> GetQuery()
        {
            return _dbSet.AsNoTracking().AsQueryable();
        }
                
        public TEntity FindById(int id)
        {
            var entity = _dbSet.Find(id);
            if (entity == null)
            {
                return null;
            }

            _context.Entry(entity).State = EntityState.Detached;
            _context.SaveChanges();

            return entity;
        }
      
        public TEntity Create(TEntity item)
        {
            if (item is IChangeLog)
            {
                FillChangeLogJson((IChangeLog)item, "Create");
            }
            var itemNew = _dbSet.Add(item).Entity;
            _context.SaveChanges();

            _context.Entry(item).State = EntityState.Detached;
            _context.SaveChanges();

            return itemNew;
        }

        public TEntity Reload(int id)
        {
            var item = _dbSet.Find(id);
            if (item == null)
            {
                return null;
            }
            _context.Entry(item).State = EntityState.Detached;
            var result = _context.Entry(item).GetDatabaseValues();

            return (TEntity)result?.ToObject();
        }

           
        public TEntity Update(TEntity item)
        {
            if (item is IChangeLog)
            {
                FillChangeLogJson((IChangeLog)item, "Update");
            }
            _context.Entry(item).State = EntityState.Modified;
            _context.SaveChanges();

            _context.Entry(item).State = EntityState.Detached;
            _context.SaveChanges();

            return item;
        }

        public void Remove(TEntity item)
        {
            try
            {
                _dbSet.Attach(item);
                _dbSet.Remove(item);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _context.Entry(item).State = EntityState.Detached;
                _context.SaveChanges();
                throw ex;
            }
        }
        private void FillChangeLogJson(IChangeLog item, string operation)
        {
            var changeLogJson = string.IsNullOrEmpty(item.ChangeLogJson) ? new List<ChangeLog>() : JsonSerializer.Deserialize<List<ChangeLog>>(item.ChangeLogJson);
            changeLogJson.Add(new ChangeLog()
            {
                Operation = String.IsNullOrEmpty(operation) ? "Update" : operation,
                Date = DateTime.Now
            });
            item.ChangeLogJson = JsonSerializer.Serialize(changeLogJson);

            while (item.ChangeLogJson.Length > 400)
            {
                var firstRecord = changeLogJson.First();
                changeLogJson.Remove(firstRecord);

                item.ChangeLogJson = JsonSerializer.Serialize(changeLogJson);
            }
        }
    }
}