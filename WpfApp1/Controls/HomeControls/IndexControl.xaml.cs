using ReactiveUI;
using System;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Controls.HomeControls;
using WpfApp1.Model;
using WpfApp1.ViewModel.HomeViewModels;

namespace WpfApp1.Controls.HomeControls
{
    /// <summary>
    /// Interaction logic for IndexControl.xaml
    /// </summary>
    public partial class IndexControl : IViewFor<IndexControlViewModel>
    {
        public IndexControl()
        {
            InitializeComponent();
            ViewModel = new IndexControlViewModel();
            DataContext = ViewModel;
            this.WhenActivated(BindView);
            // Create name scope
            NameScope.SetNameScope(RootWrapPanel, new NameScope());
        }

        public void BindView(Action<IDisposable> d)
        {
            d(ViewModel.InitializeCommand.Execute().Subscribe(AddRooms));
        }

        public void AddRooms(bool shouldInitialize)
        {
            if (!shouldInitialize || ViewModel.Rooms == null || ViewModel.Rooms.Count == 0) return;
            foreach (var room in ViewModel.Rooms)
            {
                if (RootWrapPanel.FindName($"Room{room.Name}") is RoomControl roomControl)
                    RootWrapPanel.UnregisterName($"Room{room.Name}");
                roomControl = new RoomControl(room) { Name = $"Room{room.Name}" };
                RootWrapPanel.RegisterName(roomControl.Name, roomControl);
                RootWrapPanel.Children.Add(roomControl);
            }
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (IndexControlViewModel)value;
        }

        public IndexControlViewModel ViewModel
        {
            get => (IndexControlViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(IndexControlViewModel), typeof(IndexControl));

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RootWrapPanel.ItemWidth = ActualWidth / 4;
        }
    }
}