using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ReactiveUI;
using WpfApp1.Controls.BookingControls;
using WpfApp1.Controls.ExpenseControls;
using WpfApp1.Controls.HomeControls;
using WpfApp1.Controls.ReportControls;
using WpfApp1.Controls.ServiceControls;
using WpfApp1.Model;

namespace WpfApp1.ViewModel.HomeViewModels
{
    public class MainWindowControlViewModel : ReactiveObject
    {
        public ReactiveList<MenuItem> Menu { get; set; }

        public MainWindowControlViewModel()
        {
            Menu = new ReactiveList<MenuItem>
            {
                new MenuItem() {Name = "Trang Chủ", Content = new IndexControl()},
                new MenuItem() {Name = "Quản Lý Đặt Phòng", Content = new BookingIndexControl(), MarginRequirement = new Thickness(16)},
                new MenuItem() {Name = "Xem Báo Cáo", Content = new ReportIndexControl(), MarginRequirement = new Thickness(16)},
                new MenuItem() {Name = "Quản Lý Thu Chi", Content = new ExpenseIndexControl(), MarginRequirement = new Thickness(16)},
                new MenuItem() {Name = "Quản Lý Dịch Vụ", Content = new ServiceReportControl(), MarginRequirement = new Thickness(16)}
            };
        }
    }
}