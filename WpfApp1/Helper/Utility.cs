using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using ClosedXML.Excel;
using WpfApp1.Model;
using WpfApp1.Model.Base;

namespace WpfApp1.Helper
{
    public static class Utility
    {
        private static readonly Action EmptyDelegate = delegate () { };

        public static Order CreateNewOrder(string customerId, Room room, DateTime checkInDate)
        {
            return new Order() { CustomerId = customerId, RoomId = room?.Id, RoomName = room?.Name, CheckInTime = checkInDate };
        }

        public static Orderline CreateNewOrderline(string serviceId, string orderId, double quantity, double price, string serviceName)
        {
            return new Orderline() { ServiceId = serviceId, ServiceName = serviceName, Quantity = quantity, OrderId = orderId, Price = price, Total = price * quantity };
        }

        public static void UpdateList<T>(ref List<T> list, T item) where T : ModelBase, new()
        {
            for (var i = 0; i < list.Count; i++)
            {
                if (list[i].Id == item.Id) list[i] = item;
            }
        }

        public static void ExportReport(string filePath, List<Order> data, DateTime fromDate, DateTime toDate)
        {
            if (data == null) return;
            var reportName = "Báo Cáo Tiền Phòng";
            fromDate = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day, 0, 0, 0);
            toDate = new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59);
            var columnsName = new[] { "Phòng", "Dịch Vụ", "Giảm Giá", "Phụ Thu", "Tổng Cộng" };
            DataTable dataTable = new DataTable { TableName = reportName };
            foreach (var columnName in columnsName)
            {
                dataTable.Columns.Add(columnName);
            }
            var selectedPeriod = $"Từ {fromDate:dd/MM/yyyy} Tới {toDate:dd/MM/yyyy}";
            int rowForColumnName = CreateReportHeader(dataTable, reportName, selectedPeriod);
            dataTable.Rows.Add(columnsName);
            foreach (var item in data)
            {
                var list = new List<string>();
                foreach (var ol in item.OrderLines)
                {
                    list.Add($"{ol.ServiceName} x {ol.Quantity:N} = {ol.Total:N}");
                }
                var services = String.Join(Environment.NewLine, list);
                dataTable.Rows.Add(item.Room.Name, services, $"{item.Discount:N}", $"{item.Adjustment:N}", $"{item.Total:N}");
            }
            ExportToExcel(filePath, reportName, dataTable, rowForColumnName);
        }

        public static void ExportExpense(string filePath, List<Expense> data)
        {
            if (data == null) return;
            var reportName = "Báo Cáo Thu - Chi";
            var columnsName = new[] { "Khoản Thu - Chi", "Số Tiền" };
            DataTable dataTable = new DataTable { TableName = reportName };
            foreach (var columnName in columnsName)
            {
                dataTable.Columns.Add(columnName);
            }
            var selectedPeriod = $"Ngày {DateTime.Now:dd/MM/yyyy}";
            int rowForColumnName = CreateReportHeader(dataTable, reportName, selectedPeriod);
            dataTable.Rows.Add(columnsName);
            foreach (var item in data)
            {
                dataTable.Rows.Add(item.Description, $"{item.Amount:N}");
            }
            ExportToExcel(filePath, reportName, dataTable, rowForColumnName);
        }

        public static void ExportBookings(string filePath, List<Booking> data)
        {
            if (data == null) return;
            var reportName = "Danh Sách Khách Đặt Phòng";
            var columnsName = new[] { "Tên", "Số ĐT", "Ngày Nhận Phòng", "Loại Phòng", "Ghi Chú" };
            DataTable dataTable = new DataTable { TableName = reportName };
            foreach (var columnName in columnsName)
            {
                dataTable.Columns.Add(columnName);
            }
            var selectedPeriod = $"Ngày {DateTime.Now:dd/MM/yyyy}";
            int rowForColumnName = CreateReportHeader(dataTable, reportName, selectedPeriod);
            dataTable.Rows.Add(columnsName);
            foreach (var item in data)
            {
                dataTable.Rows.Add(item.CustomerName, item.PhoneNumber, $"{item.CheckInDate:dd/MM/yyyy}", item.RoomType, item.Note);
            }
            ExportToExcel(filePath, reportName, dataTable, rowForColumnName);
        }

        private static void ExportToExcel(string filePath, string reportName, DataTable table, int rowForColumnName)
        {
            // Creating a new workbook
            var wb = new XLWorkbook();
            //Adding a worksheet
            var ws = wb.Worksheets.Add(reportName);
            // Insert data
            ws.FirstRow().FirstCell().InsertData(table.Rows);
            // merge row for full text
            var endRowInfoNumber = rowForColumnName - 1;
            var rangeInfo = ws.Range(1, 1, endRowInfoNumber, table.Columns.Count);
            for (int i = 1; i <= endRowInfoNumber; i++)
            {
                rangeInfo.Row(i).Merge();
            }
            // Format column name
            var range = ws.Range(rowForColumnName, 1, rowForColumnName, table.Columns.Count);
            range.Style.Fill.BackgroundColor = XLColor.PaleAqua;
            var endRange = ws.Range(table.Rows.Count - 2, 1, table.Rows.Count, table.Columns.Count);
            endRange.Style.Fill.BackgroundColor = XLColor.AliceBlue;
            ws.Columns().AdjustToContents();
            wb.SaveAs(filePath);
        }

        private static int CreateReportHeader(DataTable table, string reportName, string periodSelected)
        {
            table.Rows.Add(reportName);
            table.Rows.Add(periodSelected);
            return table.Rows.Count + 1;
        }

        public static void Refresh(this UIElement uiElement)
        {
            uiElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }

        public static UserControl GetParentForUserControl(UserControl control)
        {
            DependencyObject ucParent = control.Parent;

            while (ucParent != null && !(ucParent is UserControl))
            {
                ucParent = LogicalTreeHelper.GetParent(ucParent);
            }

            return (UserControl)ucParent;
        }

        public static void Print(UIElement control, string title)
        {
            PrintDialog dialog = new PrintDialog();

            //if (dialog.ShowDialog() != true) return;

            control.Measure(new Size(dialog.PrintableAreaWidth, dialog.PrintableAreaHeight));
            control.Arrange(new Rect(new Point(20, 20),
                new Size(control.DesiredSize.Width + 50, control.DesiredSize.Height + 200)));
            dialog.PrintVisual(control, title);
        }

        public static List<string> GetProvinces()
        {
            return new List<string>()
            {
                "An Giang", "Bà Rịa Vũng Tàu", "Bạc Liêu", "Bắc Cạn", "Bắc Giang", "Bắc Ninh", "Bến Tre",
                "Bình Dương", "Bình Định", "Bình Phước", "Bình Thuận", "Cà Mau", "Cao Bằng", "Cần Thơ",
                "Đà Nẵng", "Đắk Lắk", "Đắk Nông", "Điện Biên", "Đồng Nai", "Đồng Tháp", "Gia Lai", "Hà Giang",
                "Hà Nam", "Hà Nội", "Hà Tây","Hà Tĩnh", "Hải Dương", "Hải Phòng", "Hòa Bình", "Hồ Chí Minh", "Hậu Giang",
                "Hưng Yên", "Khánh Hòa", "Kiên Giang", "Kon Tum", "Lai Châu", "Lào Cai", "Lạng Sơn", "Lâm Đồng",
                "Long An", "Nam Định", "Nghệ An", "Ninh Bình", "Ninh Thuận", "Phú Thọ", "Phú Yên", "Quảng Bình", "Quảng Nam",
                "Quảng Ngãi", "Quảng Ninh", "Quảng Trị", "Sóc Trăng", "Sơn La", "Tây Ninh", "Thái Bình", "Thái Nguyên", "Thanh Hóa",
                "Thừa Thiên Huế", "Tiền Giang", "Trà Vinh", "Tuyên Quang", "Vĩnh Long", "Vĩnh Phúc", "Yên Bái"
            };
        }

        public static List<String> GetNations()
        {
            return new List<string>()
            {
                "Việt Nam",
                "Hàn Quốc",
                "Pháp",
                "Anh",
                "Ấn Độ",
                "Singapore",
                "Indonesia",
                "Thái Lan"
            };
        }

        public static List<String> GetRoomTypes()
        {
            return new List<string>()
            {
                "Phòng Đơn",
                "Phòng Đôi"
            };
        }
    }

    public enum ViewType { Day = 0, Month = 1, Year = 2 }
}