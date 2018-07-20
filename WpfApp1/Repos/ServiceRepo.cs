using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Model;
using WpfApp1.Repos.Base;

namespace WpfApp1.Repos
{
    public class ServiceRepo : RepoBase<Service>
    {
        private static List<Service> _services;

        public ServiceRepo()
        {
            _services = new List<Service>();
            Task.Run(async () =>
            {
                var services = await All();
                _services = services.ToList();
            }).Wait();
        }

        public async Task<IEnumerable<Service>> All()
        {
            var services = await GetAsync();
            if (!services.Any())
            {
                foreach (var service in SeedData.ServiceSeedData)
                {
                    service.Id = await PostAsync(service);
                    _services.Add(service);
                }
            }
            return services;
        }

        public async Task<String> Add(Service service)
        {
            var id = await PostAsync(service);
            service.Id = id;
            _services.Add(service);
            return id;
        }

        public async Task<bool> Put(Service service)
        {
            return await PutAsync(service);
        }

        public Service FindById(string id)
        {
            return _services.FirstOrDefault(x => x.Id == id);
        }

        public List<Service> GetAll() => _services;

        public Service GetRoomRateService()
        {
            return _services.FirstOrDefault(x => x.IsRoomRate);
        }
    }
}