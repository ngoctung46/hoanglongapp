using MaterialDesignThemes.Wpf;
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
using WpfApp1.Controls.CustomerControls;
using WpfApp1.Controls.OrderControls;
using WpfApp1.Controls.ServiceControls;
using WpfApp1.Model;
using WpfApp1.ViewModel.HomeViewModels;

namespace WpfApp1.Controls.HomeControls
{
    public partial class RoomControl : UserControl, IViewFor<RoomControlViewModel>
    {
        public RoomControl(Room room)
        {
            InitializeComponent();
            Room = room;
            ViewModel = new RoomControlViewModel(Room);
            DataContext = ViewModel;
            this.WhenActivated(BindView);
        }

        private void BindView(Action<IDisposable> d)
        {
            d(this.OneWayBind(ViewModel, vm => vm.IsOccupied, v => v.AvailableZone.Visibility,
                isOccupied => isOccupied ? Visibility.Collapsed : Visibility.Visible));
            d(this.OneWayBind(ViewModel, vm => vm.Status, v => v.StatusTextBlock.Foreground, status => status == Room.RoomStatus.Available ? Brushes.Green : Brushes.White));
            d(this.OneWayBind(ViewModel, vm => vm.Status, v => v.StatusTextBlock.Text,
                                status => status == Room.RoomStatus.Available ? "Phòng Trống" : "Có Khách"));
            d(this.OneWayBind(ViewModel, vm => vm.IsOccupied, v => v.OccupiedZone.Visibility,
                isOccupied => isOccupied ? Visibility.Visible : Visibility.Collapsed));
            d(this.OneWayBind(ViewModel, vm => vm.IsOccupied, v => v.RoomCard.Background, isOccupied => isOccupied ? Brushes.OrangeRed : Brushes.White));
            d(this.OneWayBind(ViewModel, vm => vm.Status, v => v.RoomGrid.Background, status => status == Room.RoomStatus.Dirty ? Brushes.DimGray : null));
            d(ViewModel.OpenDialogCommand.Subscribe(_ =>
            {
                CreateCustomerDialog();
                CustomerDialogHost.IsOpen = true;
            }));
            d(ViewModel.ShowCustomerInfoCommand.Subscribe(customer =>
            {
                if (customer == null) return;
                customer.RoomName = Room.Name;
                DialogContentPanel.Children.Clear();
                var control = new CustomerInfoControl(customer) { Height = Double.NaN, Width = 500, DialogHost = CustomerDialogHost };
                DialogContentPanel.Children.Add(control);
                CustomerDialogHost.IsOpen = true;
            }));
            d(ViewModel.ViewReceiptCommand.Subscribe(_ =>
            {
                DialogContentPanel.Children.Clear();
                var control = new OrderControl(Room) { DialogHost = CustomerDialogHost, Owner = this };
                DialogContentPanel.Children.Add(control);
                CustomerDialogHost.IsOpen = true;
            }));
            d(ViewModel.AddServiceCommand.Subscribe(_ =>
            {
                DialogContentPanel.Children.Clear();
                var control = new ServiceControl(Room.OrderId) { Height = Double.NaN, Width = 500, DialogHost = CustomerDialogHost };
                DialogContentPanel.Children.Add(control);
                CustomerDialogHost.IsOpen = true;
            }));
            d(ViewModel.ChangeRoomCommand.Subscribe(_ =>
            {
                DialogContentPanel.Children.Clear();
                var control = new ChangeRoomControl(ViewModel) { Height = Double.NaN, Width = Double.NaN, DialogHost = CustomerDialogHost };
                DialogContentPanel.Children.Add(control);
                CustomerDialogHost.IsOpen = true;
            }));
        }

        internal CustomerControl CreateCustomerDialog()
        {
            DialogContentPanel.Children.Clear();
            var customerControl = new CustomerControl(ViewModel)
            {
                Height = Double.NaN,
                Width = 550,
                DialogHost = CustomerDialogHost
            };
            DialogContentPanel.Children.Add(customerControl);
            return customerControl;
        }

        public static DependencyProperty RoomProperty = DependencyProperty.Register("Room", typeof(Room), typeof(RoomControl));

        public Room Room
        {
            get => (Room)GetValue(RoomProperty);
            set => SetValue(RoomProperty, value);
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (RoomControlViewModel)value;
        }

        public RoomControlViewModel ViewModel
        {
            get => (RoomControlViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(RoomControlViewModel), typeof(RoomControl));
    }
}