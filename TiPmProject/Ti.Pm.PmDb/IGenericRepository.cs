namespace Ti.Pm.PmDb.Model
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        TEntity Create(TEntity item);
        TEntity FindById(int id);
        IEnumerable<TEntity> Get();
        IQueryable<TEntity> GetQuery();       
        void Remove(TEntity item);       
        void SetUserNameForLog(string userName);       

    }
}
