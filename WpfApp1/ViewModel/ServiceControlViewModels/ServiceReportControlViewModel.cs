using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Controls.ServiceControls;
using WpfApp1.Model;
using WpfApp1.Repos;

namespace WpfApp1.ViewModel.ServiceControlViewModels
{
    public class ServiceReportControlViewModel : ReactiveObject
    {
        public ReactiveList<Service> Services { get; set; }
        private readonly ServiceRepo _serviceRepo;
        public ReactiveCommand<Unit, Unit> RefreshCommand { get; set; }
        public ReactiveCommand<Unit, Unit> StockCommand { get; set; }

        public ServiceReportControlViewModel()
        {
            _serviceRepo = new ServiceRepo();
            Services = new ReactiveList<Service>(GetServices()) { ChangeTrackingEnabled = true };
            RefreshCommand = ReactiveCommand.Create(Refresh);
            StockCommand = ReactiveCommand.Create(Stock);
        }

        private void Refresh()
        {
            Services = new ReactiveList<Service>(GetServices()) { ChangeTrackingEnabled = true };
        }

        private void Stock()
        {
            var serviceWindow = new ServiceWindow();
            serviceWindow.ContentControl.Content = new ServiceControl();
            serviceWindow.ShowDialog();
        }

        internal IEnumerable<Service> GetServices() => _serviceRepo.GetAll().Where(x => !x.IsRoomRate);
    }
}