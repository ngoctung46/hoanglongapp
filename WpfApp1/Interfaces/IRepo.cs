using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Model.Base;

namespace WpfApp1.Interfaces
{
    public interface IRepo<T> where T : ModelBase
    {
        Task<String> PostAsync(T entity);

        Task<bool> PutAsync(T entity);

        Task<bool> DeleteAsync(string id);

        Task<IEnumerable<T>> GetAsync();
    }
}