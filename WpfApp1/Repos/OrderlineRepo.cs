using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Model;
using WpfApp1.Repos.Base;

namespace WpfApp1.Repos
{
    public class OrderlineRepo : RepoBase<Orderline>
    {
        private static List<Orderline> _orderLines;

        public OrderlineRepo()
        {
            Task.Run(async () =>
            {
                var orderlines = await All();
                _orderLines = orderlines.ToList();
            }).Wait();
        }

        public async Task<IEnumerable<Orderline>> All()
        {
            var orderlines = await GetAsync();
            return orderlines;
        }

        public async Task<String> Add(Orderline orderline)
        {
            var id = await PostAsync(orderline);
            orderline.Id = id;
            _orderLines.Add(orderline);
            return id;
        }

        public IEnumerable<Orderline> FindByOrderId(string orderId)
        {
            return _orderLines.FindAll(x => x.OrderId == orderId);
        }
    }
}