using System;
using System.Linq;

namespace GithubTFSBridge.Providers
{
    public interface IIssuesProvider<T>
    {
        void Insert(T entity);
        void Delete(T entity);
        IQueryable<T> SearchFor(Func<T, bool> predicate);
        IQueryable<T> GetAll();
        T GetById(string id);
    }
}
