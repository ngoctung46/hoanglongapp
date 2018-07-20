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
using MaterialDesignThemes.Wpf;
using ReactiveUI;
using WpfApp1.Controls.OrderControls;
using WpfApp1.Model;
using WpfApp1.ViewModel.ServiceControlViewModels;

namespace WpfApp1.Controls.ServiceControls
{
    /// <summary>
    /// Interaction logic for ServiceControl.xaml
    /// </summary>
    public partial class ServiceControl : UserControl, IViewFor<ServiceControlViewModel>
    {
        public ServiceControl(string orderId = "")
        {
            OrderId = orderId;
            InitializeComponent();
            if (orderId == "") CancelButton.Visibility = Visibility.Collapsed;
            else CancelButton.Visibility = Visibility.Visible;
            ViewModel = new ServiceControlViewModel(OrderId);
            DataContext = ViewModel;
            this.WhenActivated(BindView);
            NameComboBox.ItemsSource = ViewModel.Services;
        }

        private void BindView(Action<IDisposable> d)
        {
            d(ViewModel.CloseCommand.Subscribe(_ =>
            {
                if (DialogHost != null) DialogHost.IsOpen = false;
            }));
            d(ViewModel.AddCommand.Subscribe(_ =>
            {
                if (DialogHost != null) DialogHost.IsOpen = false;
            }));
        }

        public static DependencyProperty OrderProperty = DependencyProperty.Register("Order", typeof(string), typeof(ServiceControl));

        public static DependencyProperty DialogHostProperty = DependencyProperty.Register("DialogHost", typeof(DialogHost), typeof(ServiceControl));

        public DialogHost DialogHost
        {
            get => (DialogHost)GetValue(DialogHostProperty);
            set => SetValue(DialogHostProperty, value);
        }

        public string OrderId
        {
            get => (string)GetValue(OrderProperty);
            set => SetValue(OrderProperty, value);
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (ServiceControlViewModel)value;
        }

        public ServiceControlViewModel ViewModel
        {
            get => (ServiceControlViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(ServiceControlViewModel), typeof(ServiceControl));
    }
}