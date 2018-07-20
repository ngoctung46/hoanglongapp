using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Helper;
using WpfApp1.Model;
using WpfApp1.Model.Base;
using WpfApp1.Repos.Base;

namespace WpfApp1.Repos
{
    public class OrderRepo : RepoBase<Order>
    {
        private static List<Order> _orders;
        private readonly OrderlineRepo _orderlineRepo;

        public OrderRepo()
        {
            _orderlineRepo = new OrderlineRepo();
            Task.Run(async () =>
            {
                var orders = await All();
                _orders = orders.ToList();
            }).Wait();
        }

        public async Task<IEnumerable<Order>> All()
        {
            var orders = await GetAsync();
            return orders;
        }

        public async Task<String> Add(Order order)
        {
            var id = await PostAsync(order);
            order.Id = id;
            _orders.Add(order);
            return id;
        }

        public async Task<bool> Update(Order order)
        {
            var success = await PutAsync(order);
            if (success)
            {
                Utility.UpdateList(ref _orders, order);
            }
            return success;
        }

        public IEnumerable<Orderline> GetOrderlines(string orderId)
        {
            return _orderlineRepo.FindByOrderId(orderId);
        }

        public Order FindOneById(string orderId)
            => _orders.FirstOrDefault(x => x.Id == orderId);

        public IEnumerable<Order> GetAll() => _orders;
    }
}