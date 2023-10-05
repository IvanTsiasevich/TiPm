using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Ti.Pm.PmDb.Model;

namespace Ti.Pm.PmDb
{
    public class EFRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private DbContext mContext;
        private DbSet<TEntity> mDbSet;
        private string mUser;

        public EFRepository(DbContext context,  string user = "" )
        {
            mContext = context;
            mDbSet = context.Set<TEntity>();
            mUser = user;
        }
        public void SetUserNameForLog(string user)
        {
            mUser = user;
        }

        public IEnumerable<TEntity> Get()
        {
            return mDbSet.AsNoTracking().ToList();
        }

        public IQueryable<TEntity> GetQuery()
        {
            return mDbSet.AsNoTracking().AsQueryable();
        }
                
        public TEntity FindById(int id)
        {
            var entity = mDbSet.Find(id);
            if (entity == null)
            {
                return null;
            }

            mContext.Entry(entity).State = EntityState.Detached;
            mContext.SaveChanges();

            return entity;
        }
      
        public TEntity Create(TEntity item)
        {
            if (item is IChangeLog)
            {
                FillChangeLogJson((IChangeLog)item, "Create");
            }
            var itemNew = mDbSet.Add(item).Entity;
            mContext.SaveChanges();

            mContext.Entry(item).State = EntityState.Detached;
            mContext.SaveChanges();

            return itemNew;
        }

        public TEntity Reload(int id)
        {
            var item = mDbSet.Find(id);
            if (item == null)
            {
                return null;
            }
            mContext.Entry(item).State = EntityState.Detached;
            var result = mContext.Entry(item).GetDatabaseValues();

            return (TEntity)result?.ToObject();
        }

           
        public TEntity Update(TEntity item)
        {
            if (item is IChangeLog)
            {
                FillChangeLogJson((IChangeLog)item, "Update");
            }
            mContext.Entry(item).State = EntityState.Modified;
            mContext.SaveChanges();

            mContext.Entry(item).State = EntityState.Detached;
            mContext.SaveChanges();

            return item;
        }

        public void Remove(TEntity item)
        {
            try
            {
                mDbSet.Attach(item);
                mDbSet.Remove(item);
                mContext.SaveChanges();
            }
            catch (Exception ex)
            {
                mContext.Entry(item).State = EntityState.Detached;
                mContext.SaveChanges();
                throw ex;
            }
        }
        private void FillChangeLogJson(IChangeLog item, string operation)
        {
            var changeLogJson = string.IsNullOrEmpty(item.ChangeLogJson) ? new List<ChangeLog>() : JsonSerializer.Deserialize<List<ChangeLog>>(item.ChangeLogJson);
            changeLogJson.Add(new ChangeLog()
            {
                Operation = String.IsNullOrEmpty(operation) ? "Update" : operation,
                User = mUser,
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