using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Helper;
using WpfApp1.Model;
using WpfApp1.Repos.Base;

namespace WpfApp1.Repos
{
    public class CustomerRepo : RepoBase<Customer>
    {
        private static List<Customer> _customers;

        public CustomerRepo()
        {
            Task.Run(async () =>
            {
                var customers = await All();
                _customers = customers.ToList();
            }).Wait();
        }

        public async Task<String> Add(Customer customer)
        {
            var id = await PostAsync(customer);
            customer.Id = id;
            _customers.Add(customer);
            return id;
        }

        public async Task<bool> Update(Customer customer)
        {
            var success = await PutAsync(customer);
            if (success)
            {
                Utility.UpdateList(ref _customers, customer);
            }
            return success;
        }

        public async Task<IEnumerable<Customer>> All()
        {
            var customers = await GetAsync();
            return customers;
        }

        public Customer FindByIdentity(string identity)
        {
            return _customers.FirstOrDefault(x => x.Identity == identity);
        }

        public Customer FindById(string id)
        {
            return _customers.FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<Customer> GetAll() => _customers;
    }
}