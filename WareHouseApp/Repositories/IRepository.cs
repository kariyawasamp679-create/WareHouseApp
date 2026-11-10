using System.Data;
using WareHouseApp.Models;

namespace WareHouseApp.Repositories
{
    /// <summary>Contract every data repository fulfils (polymorphism: UI works against this).</summary>
    public interface IRepository<T> where T : Entity
    {
        DataTable Search(string keyword);
        void Add(T item);
        void Update(T item);
        void Delete(int id);
        int Count();
    }
}
