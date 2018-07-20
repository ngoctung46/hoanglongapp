using MaterialDesignThemes.Wpf;
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
using WpfApp1.Model;

namespace WpfApp1.Controls.CustomerControls
{
    /// <summary>
    /// Interaction logic for CustomerInfoControl.xaml
    /// </summary>
    public partial class CustomerInfoControl
    {
        public CustomerInfoControl(Customer customer)
        {
            InitializeComponent();
            DataContext = customer;
        }

        public static DependencyProperty DialogHostProperty =
            DependencyProperty.Register("DialogHost", typeof(DialogHost), typeof(CustomerInfoControl));

        public DialogHost DialogHost
        {
            get => (DialogHost)GetValue(DialogHostProperty);
            set => SetValue(DialogHostProperty, value);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            DialogHost.IsOpen = false;
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            Utility.Print(PrintGrid, "Customer Info");
            DialogHost.IsOpen = false;
        }
    }
}