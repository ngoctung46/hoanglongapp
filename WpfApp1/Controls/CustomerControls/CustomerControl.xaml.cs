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
using ReactiveUI;
using WpfApp1.Controls.HomeControls;
using WpfApp1.ViewModel.CustomerViewModels;
using WpfApp1.ViewModel.HomeViewModels;
using MaterialDesignThemes.Wpf;
using WpfApp1.Helper;
using WpfApp1.Model;

namespace WpfApp1.Controls.CustomerControls
{
    /// <summary>
    /// Interaction logic for CustomerControl.xaml
    /// </summary>
    public partial class CustomerControl : UserControl, IViewFor<CustomerControlViewModel>
    {
        public CustomerControl(RoomControlViewModel roomControlViewModel)
        {
            InitializeComponent();
            this.WhenActivated(BindView);
            ViewModel = new CustomerControlViewModel(roomControlViewModel);
            DataContext = ViewModel;
            IdComboBox.ItemsSource = ViewModel.Customers.Select(x => x.Identity).ToList();
            var provinces = Utility.GetProvinces();
            var countries = Utility.GetNations();
            IssuePlaceComboBox.ItemsSource = provinces;
            BirthPlaceComboBox.ItemsSource = provinces;
            CitiesComboBox.ItemsSource = provinces;
            CountriesComboBox.ItemsSource = countries;
        }

        private void BindView(Action<IDisposable> d)
        {
            d(this.BindCommand(ViewModel, vm => vm.SearchCustomerCommand, v => v.IdComboBox, nameof(IdComboBox.LostFocus)));
            d(this.OneWayBind(ViewModel, vm => vm.IsNewCustomer, v => v.IdComboBox.IsEnabled));
            d(this.OneWayBind(ViewModel, vm => vm.IsNewCustomer, v => v.CustomerNameTextBox.IsEnabled));
            d(this.OneWayBind(ViewModel, vm => vm.IsNewCustomer, v => v.BirthInfoStackPanel.IsEnabled));
            d(this.OneWayBind(ViewModel, vm => vm.IsNewCustomer, v => v.IdentityInfoStackPanel.IsEnabled));
            d(this.OneWayBind(ViewModel, vm => vm.IsNewCustomer, v => v.AddressInfoStackPanel.IsEnabled));
            d(ViewModel.AcceptCommand.Subscribe(_ =>
            {
                if (Window.GetWindow(this) is MainWindow mainWindow)
                {
                    var control = mainWindow.MainWindowControl.ContentControl.Content;
                    if (control is IndexControl indexControl)
                        indexControl.ViewModel.AvailableCount--;
                }
                DialogHost.IsOpen = false;
            }));
            d(ViewModel.SearchCustomerCommand.Subscribe(_ => IdComboBox.IsEnabled = false));
        }

        public static DependencyProperty DialogHostProperty = DependencyProperty.Register("DialogHost", typeof(DialogHost), typeof(CustomerControl));

        public DialogHost DialogHost
        {
            get => (DialogHost)GetValue(DialogHostProperty);
            set => SetValue(DialogHostProperty, value);
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (CustomerControlViewModel)value;
        }

        public CustomerControlViewModel ViewModel
        {
            get => (CustomerControlViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(CustomerControlViewModel), typeof(CustomerControl));
    }
}