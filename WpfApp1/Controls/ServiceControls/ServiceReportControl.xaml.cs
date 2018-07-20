using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp1.ViewModel.ServiceControlViewModels;

namespace WpfApp1.Controls.ServiceControls
{
    /// <summary>
    /// Interaction logic for ServiceReportControl.xaml
    /// </summary>
    public partial class ServiceReportControl : UserControl, IViewFor<ServiceReportControlViewModel>
    {
        public ServiceReportControl()
        {
            InitializeComponent();
            ViewModel = new ServiceReportControlViewModel();
            this.WhenActivated(Bind);
        }

        private void Bind(Action<IDisposable> d)
        {
            d(this.OneWayBind(ViewModel, vm => vm.Services, v => v.ServiceReportListView.ItemsSource));
            d(this.BindCommand(ViewModel, vm => vm.RefreshCommand, v => v.RefreshButton));
            d(this.BindCommand(ViewModel, vm => vm.StockCommand, v => v.StockButton));
            d(this.WhenAnyObservable(x => x.ViewModel.RefreshCommand).Subscribe(_ =>
            {
                ServiceReportListView.ItemsSource = ViewModel.Services;
                ServiceReportListView.Items.Refresh();
            }));
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (ServiceReportControlViewModel)value;
        }

        public ServiceReportControlViewModel ViewModel
        {
            get => (ServiceReportControlViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(ServiceReportControlViewModel), typeof(ServiceReportControl));
    }
}