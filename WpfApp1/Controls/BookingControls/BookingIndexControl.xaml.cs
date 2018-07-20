using Microsoft.Win32;
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
using WpfApp1.Helper;
using WpfApp1.ViewModel.BookingVIewModels;

namespace WpfApp1.Controls.BookingControls
{
    /// <summary>
    /// Interaction logic for BookingIndexControl.xaml
    /// </summary>
    public partial class BookingIndexControl : UserControl, IViewFor<BookingIndexControlViewModel>
    {
        public BookingIndexControl()
        {
            InitializeComponent();
            ViewModel = new BookingIndexControlViewModel();
            DataContext = ViewModel;
            SetDefault();
            this.WhenActivated(d =>
            {
                d(this.OneWayBind(ViewModel, vm => vm.Bookings.Count, v => v.BookingListView.Visibility,
                    count => count > 0 ? Visibility.Visible : Visibility.Collapsed));
                d(ViewModel.FilePath.RegisterHandler(interaction =>
                {
                    SaveFileDialog dialog = new SaveFileDialog
                    {
                        Filter = "Excel files(.xlsx)| *.xlsx",
                        DefaultExt = ".xlsx",
                        Title = "Thư Mục Tới"
                    };
                    var result = dialog.ShowDialog();
                    if (result == true && dialog.FileName != "null")
                    {
                        interaction.SetOutput(dialog.FileName);
                    }
                }));
            });
        }

        private void SetDefault()
        {
            var roomTypes = Utility.GetRoomTypes();
            RoomTypeComboBox.ItemsSource = roomTypes;
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (BookingIndexControlViewModel)value;
        }

        public BookingIndexControlViewModel ViewModel
        {
            get => (BookingIndexControlViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(BookingIndexControlViewModel), typeof(BookingIndexControl));

        private void BookingListView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                ViewModel.DeleteCommand.Execute().Subscribe();
            }
        }
    }
}