namespace HostelDB.DbRepository
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        TEntity Create(TEntity item, string operation = "");
        TEntity FindById(int id);
        IEnumerable<TEntity> Get();
        IQueryable<TEntity> GetQuery();
        IEnumerable<TEntity> Get(Func<TEntity, bool> predicate);
        IEnumerable<TEntity> Take(int count);
        IEnumerable<TEntity> Take(int count, Func<TEntity, bool> predicate);
        void Remove(TEntity item);
        TEntity Update(TEntity item, string operation = "");
        void SetUserNameForLog(string user);
    }
}
