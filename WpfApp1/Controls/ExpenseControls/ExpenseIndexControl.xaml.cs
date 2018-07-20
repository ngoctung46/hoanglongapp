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
using Microsoft.Win32;
using ReactiveUI;
using WpfApp1.ViewModel.ExpenseViewModels;

namespace WpfApp1.Controls.ExpenseControls
{
    /// <summary>
    /// Interaction logic for ExpenseIndexControl.xaml
    /// </summary>
    public partial class ExpenseIndexControl : UserControl, IViewFor<ExpenseIndexControlViewModel>
    {
        public ExpenseIndexControl()
        {
            InitializeComponent();
            ViewModel = new ExpenseIndexControlViewModel();
            DataContext = ViewModel;
            this.WhenActivated(d =>
            {
                d(this.OneWayBind(ViewModel, vm => vm.Total, v => v.TotalTextBlock.Visibility,
                    total => total != 0 ? Visibility.Visible : Visibility.Collapsed));
                d(this.OneWayBind(ViewModel, vm => vm.Expenses.Count, v => v.ExpenseListView.Visibility,
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
            set => ViewModel = (ExpenseIndexControlViewModel)value;
        }

        public ExpenseIndexControlViewModel ViewModel
        {
            get => (ExpenseIndexControlViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(ExpenseIndexControlViewModel), typeof(ExpenseIndexControl));

        private void ExpenseListView_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                ViewModel.DeleteCommand.Execute().Subscribe();
            }
        }
    }
}