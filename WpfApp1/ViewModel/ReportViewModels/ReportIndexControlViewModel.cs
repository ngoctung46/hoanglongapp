using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using WpfApp1.Helper;
using WpfApp1.Model;
using WpfApp1.Repos;

namespace WpfApp1.ViewModel.ReportViewModels
{
    public class ReportIndexControlViewModel : ReactiveObject
    {
        private readonly OrderRepo _orderRepo;
        private readonly RoomRepo _roomRepo;
        private readonly ServiceRepo _serviceRepo;
        private readonly ExpenseRepo _expenseRepo;
        private readonly Interaction<Unit, string> filePath;
        private DateTime _fromDate = DateTime.Now;
        private ViewType _viewType = ViewType.Day;

        public Interaction<Unit, string> FilePath => filePath;

        public DateTime FromDate
        {
            get => _fromDate;
            set => this.RaiseAndSetIfChanged(ref _fromDate, value);
        }

        public ViewType ViewType
        {
            get => _viewType;
            set => this.RaiseAndSetIfChanged(ref _viewType, value);
        }

        private DateTime _toDate = DateTime.Now;

        public DateTime ToDate
        {
            get => _toDate;
            set => this.RaiseAndSetIfChanged(ref _toDate, value);
        }

        public ReactiveList<Order> Orders { get; set; }
        public ReactiveCommand<Unit, Unit> ViewCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> ExportCommand { get; protected set; }

        public ReportIndexControlViewModel()
        {
            _orderRepo = new OrderRepo();
            _roomRepo = new RoomRepo();
            _serviceRepo = new ServiceRepo();
            _expenseRepo = new ExpenseRepo();
            filePath = new Interaction<Unit, string>();
            Orders = new ReactiveList<Order>();
            ViewCommand = ReactiveCommand.Create(GetReports);
            ExportCommand = ReactiveCommand.Create(Export);
        }

        private void GetReports()
        {
            Orders.Clear();
            FromDate = new DateTime(FromDate.Year, FromDate.Month, FromDate.Day, 0, 0, 0);
            ToDate = new DateTime(ToDate.Year, ToDate.Month, ToDate.Day, 23, 59, 59);
            var orders = _orderRepo.GetAll().Where(x => x.CheckOutTime.ToLocalTime() <= ToDate && x.CheckOutTime.ToLocalTime() >= FromDate);

            //group by month
            if (ViewType == ViewType.Month)
            {
                orders = orders.GroupBy(x => new { x.Room.Name, x.CheckOutTime.Month }).Select(x => new Order
                {
                    Room = x.First().Room,
                    Discount = x.Sum(y => y.Discount),
                    Adjustment = x.Sum(z => z.Adjustment),
                    Total = x.Sum(t => t.Total)
                });
            }

            if (ViewType == ViewType.Year)
            {
                orders = orders.GroupBy(x => x.CheckOutTime.Year).Select(x => new Order
                {
                    Room = x.First().Room,
                    Discount = x.Sum(y => y.Discount),
                    Adjustment = x.Sum(z => z.Adjustment),
                    Total = x.Sum(t => t.Total)
                });
            }
            if (!orders.Any()) return;

            foreach (var order in orders)
            {
                var room = _roomRepo.GetById(order.RoomId);
                order.OrderLines = GetOrderLines(order.Id);
                Orders.Add(new Order()
                {
                    Id = order.Id,
                    OrderLines = order.OrderLines,
                    Total = order.Total,
                    Room = room,
                    Discount = order.Discount,
                    Adjustment = order.Adjustment
                });
            }
            // Add total row
            var total = Orders.Sum(x => x.Total);
            var discount = Orders.Sum(x => x.Discount);
            var adjustment = Orders.Sum(x => x.Adjustment);
            Orders.Add(new Order()
            {
                Total = total,
                Discount = discount,
                Adjustment = adjustment,
                Room = new Room() { Name = "Tổng Tiền Phòng" }
            });
            // Add Total Expense Row
            var totalExpense = _expenseRepo.GetAll()
                .Where(x => x.CreatedAt.ToLocalTime() >= FromDate && x.CreatedAt.ToLocalTime() <= ToDate)
                .Sum(x => x.Amount);
            Orders.Add(new Order()
            {
                Total = totalExpense,
                Room = new Room() { Name = "Tổng Thu Chi" }
            });

            // Add last row
            var actualBalance = total + totalExpense;
            Orders.Add(new Order()
            {
                Total = actualBalance,
                Room = new Room() { Name = "Tiền Mặt Hiện Có" }
            });
        }

        private List<Orderline> GetOrderLines(string orderId)
        {
            var list = _orderRepo.GetOrderlines(orderId).ToList();
            var orderLines = list;
            foreach (var orderLine in orderLines)
            {
                orderLine.Service = GetService(orderLine.ServiceId);
            }
            return orderLines;
        }

        private Service GetService(string serviceId)
        {
            return _serviceRepo.FindById(serviceId);
        }

        private async void Export()
        {
            try
            {
                var filePath = await this.filePath.Handle(Unit.Default);
                Utility.ExportReport(filePath, Orders.ToList(), FromDate, ToDate);
            }
            catch (Exception ex)
            {
                return;
            }
        }
    }
}