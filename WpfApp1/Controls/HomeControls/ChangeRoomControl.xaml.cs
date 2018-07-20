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
using MaterialDesignThemes.Wpf;
using WpfApp1.Controls.ServiceControls;
using WpfApp1.Helper;
using WpfApp1.ViewModel.HomeViewModels;
using WpfApp1.ViewModel.ServiceControlViewModels;
using WpfApp1.Model;
using WpfApp1.Repos;

namespace WpfApp1.Controls.HomeControls
{
    /// <summary>
    /// Interaction logic for ChangeRoomControl.xaml
    /// </summary>
    public partial class ChangeRoomControl : UserControl, IViewFor<ChangeRoomControlViewModel>
    {
        private RoomControlViewModel _oldRoomVM;

        public ChangeRoomControl(RoomControlViewModel roomVM)
        {
            InitializeComponent();
            _oldRoomVM = roomVM;
            ViewModel = new ChangeRoomControlViewModel(roomVM.Room);
            DataContext = ViewModel;
            RoomComboBox.ItemsSource = ViewModel.Rooms;
            this.WhenActivated(BindView);
        }

        private void BindView(Action<IDisposable> d)
        {
            d(ViewModel.CancelCommand.Subscribe(_ => DialogHost.IsOpen = false));
            d(ViewModel.AcceptCommand.Subscribe(_ =>
            {
                RestartApp();
            }));
        }

        private void RestartApp()
        {
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }

        public static DependencyProperty DialogHostProperty = DependencyProperty.Register("DialogHost", typeof(DialogHost), typeof(ChangeRoomControl));

        public DialogHost DialogHost
        {
            get => (DialogHost)GetValue(DialogHostProperty);
            set => SetValue(DialogHostProperty, value);
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (ChangeRoomControlViewModel)value;
        }

        public ChangeRoomControlViewModel ViewModel
        {
            get => (ChangeRoomControlViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(ChangeRoomControlViewModel), typeof(ChangeRoomControl));
    }
}