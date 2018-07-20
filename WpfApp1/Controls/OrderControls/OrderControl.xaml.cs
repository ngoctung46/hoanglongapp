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
using WpfApp1.Controls.CustomerControls;
using WpfApp1.Controls.HomeControls;
using WpfApp1.Helper;
using WpfApp1.Model;
using WpfApp1.ViewModel.OrderViewModels;

namespace WpfApp1.Controls.OrderControls
{
    /// <summary>
    /// Interaction logic for OrderControl.xaml
    /// </summary>
    public partial class OrderControl : UserControl, IViewFor<OrderControlViewModel>
    {
        public OrderControl(Room room)
        {
            Room = room;
            ViewModel = new OrderControlViewModel(room);
            DataContext = ViewModel;
            InitializeComponent();
            RoomTextBlock.Text = room.Name;
            this.WhenActivated(BindView);
        }

        private void BindView(Action<IDisposable> d)
        {
            d(ViewModel.CloseCommand.Subscribe(_ => DialogHost.IsOpen = false));
            d(ViewModel.CheckOutCommand.Subscribe(_ =>
            {
                MessageBox.Show("NHỚ TRẢ GIẤY TỜ CHO KHÁCH !!!", "Lưu Ý!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                Print_Click(null, null);
                if (Owner is RoomControl roomControl)
                    roomControl.ViewModel.CheckOutCommand.Execute().Subscribe();
                if (Window.GetWindow(this) is MainWindow mainWindow)
                {
                    var control = mainWindow.MainWindowControl.ContentControl.Content;
                    if (control is IndexControl indexControl)
                        indexControl.ViewModel.AvailableCount++;
                }
                DialogHost.IsOpen = false;
            }));
        }

        public static DependencyProperty RoomProperty = DependencyProperty.Register("Room", typeof(Room), typeof(OrderControl));

        public static DependencyProperty DialogHostProperty = DependencyProperty.Register("DialogHost", typeof(DialogHost), typeof(OrderControl));

        public static DependencyProperty OwnerProperty = DependencyProperty.Register("Owner", typeof(UserControl), typeof(OrderControl));

        public DialogHost DialogHost
        {
            get => (DialogHost)GetValue(DialogHostProperty);
            set => SetValue(DialogHostProperty, value);
        }

        public Room Room
        {
            get => (Room)GetValue(RoomProperty);
            set => SetValue(RoomProperty, value);
        }

        public UserControl Owner
        {
            get => (UserControl)GetValue(OwnerProperty);
            set => SetValue(OwnerProperty, value);
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (OrderControlViewModel)value;
        }

        public OrderControlViewModel ViewModel
        {
            get => (OrderControlViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(OrderControlViewModel), typeof(OrderControl));

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            Utility.Print(PrintStackPanel, "Receipt");
        }
    }
}