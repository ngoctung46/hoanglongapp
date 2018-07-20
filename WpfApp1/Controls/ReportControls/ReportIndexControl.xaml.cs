using Microsoft.Win32;
using ReactiveUI;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.ViewModel.ReportViewModels;

namespace WpfApp1.Controls.ReportControls
{
    /// <summary>
    /// Interaction logic for ReportIndexControl.xaml
    /// </summary>
    public partial class ReportIndexControl : UserControl, IViewFor<ReportIndexControlViewModel>
    {
        public ReportIndexControl()
        {
            InitializeComponent();
            ViewModel = new ReportIndexControlViewModel();
            DataContext = ViewModel;
            this.WhenActivated(d =>
            {
                d(this.OneWayBind(ViewModel, vm => vm.Orders, v => v.ReportListView.ItemsSource));
                d(this.OneWayBind(ViewModel, vm => vm.Orders.Count, vm => vm.ReportListView.Visibility,
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

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (ReportIndexControlViewModel)value;
        }

        public ReportIndexControlViewModel ViewModel
        {
            get => (ReportIndexControlViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(ReportIndexControlViewModel), typeof(ReportIndexControl));

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}