using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using WpfApp1.Helper;
using WpfApp1.Model;
using WpfApp1.Repos;

namespace WpfApp1.ViewModel.OrderViewModels
{
    public class OrderControlViewModel : ReactiveObject
    {
        private readonly Room _room;
        private readonly ServiceRepo _serviceRepo;
        private readonly OrderlineRepo _orderlineRepo;
        private readonly CustomerRepo _customerRepo;
        private readonly OrderRepo _orderRepo;
        private readonly ObservableAsPropertyHelper<double> _total;
        public double Total => _total.Value;

        private DateTime _checkInTime;

        public DateTime CheckInTime
        {
            get => _checkInTime;
            set => this.RaiseAndSetIfChanged(ref _checkInTime, value);
        }

        private double _discount;

        public double Discount
        {
            get => _discount;
            set => this.RaiseAndSetIfChanged(ref _discount, value);
        }

        private double _adjustment;

        public double Adjustment
        {
            get => _adjustment;
            set => this.RaiseAndSetIfChanged(ref _adjustment, value);
        }

        private double _subTotal;

        public double Subtotal
        {
            get => _subTotal;
            set => this.RaiseAndSetIfChanged(ref _subTotal, value);
        }

        public ReactiveList<Orderline> Orderlines { get; set; }
        public ReactiveCommand<Unit, Unit> CloseCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> CheckOutCommand { get; protected set; }

        public OrderControlViewModel(Room room)
        {
            _serviceRepo = new ServiceRepo();
            _orderlineRepo = new OrderlineRepo();
            _customerRepo = new CustomerRepo();
            _orderRepo = new OrderRepo();
            _room = room;
            Orderlines = new ReactiveList<Orderline>();
            GetOrderInfo();
            CloseCommand = ReactiveCommand.Create(Close);
            CheckOutCommand = ReactiveCommand.CreateFromTask(CheckOut);
            _total = this.WhenAnyValue(x => x.Adjustment, y => y.Discount, z => z.Subtotal,
                (adjustment, discount, subTotal) => subTotal + adjustment - discount).ToProperty(this, x => x.Total);
        }

        private void GetOrderInfo()
        {
            var checkInTime = _orderRepo.FindOneById(_room.OrderId).CheckInTime.ToLocalTime();
            CheckInTime = checkInTime;
            var fee = CalculateFee(checkInTime);
            if (_room == null) return;
            var orderlines = _orderlineRepo.FindByOrderId(_room?.OrderId);
            Subtotal = 0.0;
            var roomService = _serviceRepo.GetRoomRateService();
            if (roomService == null) return;
            roomService.CurrentPrice = _room.Rate;
            if (fee > 0)
            {
                if (fee < 350_000) roomService.CurrentPrice = fee;
                var quantity = (int)fee / _room.Rate;
                quantity = quantity >= 1 ? quantity : 1;
                Orderlines.Add(new Orderline()
                {
                    ServiceId = roomService.Id,
                    Service = roomService,
                    ServiceName = roomService.Name,
                    OrderId = _room.OrderId,
                    Quantity = quantity,
                    Price = fee,
                    Total = fee
                });
            }
            Orderlines.AddRange(orderlines);
            Subtotal = Orderlines.Sum(x => x.Total);
            var orderQuantity = Orderlines.Sum(x => x.Quantity);

            Orderlines.Add(new Orderline() { ServiceName = "Tổng", Total = Subtotal, Quantity = orderQuantity });
        }

        private void Close()
        {
        }

        private async Task CheckOut()
        {
            var order = new Order
            {
                Id = _room?.OrderId,
                Total = Total,
                CheckInTime = CheckInTime,
                CheckOutTime = DateTime.Now,
                Discount = Discount,
                Adjustment = Adjustment,
                RoomId = _room?.Id,
                RoomName = _room.Name
            };
            await _orderRepo.Update(order);
            await _orderlineRepo.Add(Orderlines[0]);
        }

        private double CalculateFee(DateTime checkInTime)
        {
            var checkOutTime = DateTime.Now;
            var stayedTime = Math.Round(checkOutTime.Subtract(checkInTime).TotalHours, 2);
            if (stayedTime >= 0 && stayedTime < 4) return CalculateHourlyFee(stayedTime);
            return CalculateDailyFee(checkInTime);
        }

        private double CalculateHourlyFee(double stayedTime)
        {
            if (stayedTime > 3.2) return _room.Type == Room.RoomType.Single ? 210_000 : 250_000;
            if (stayedTime > 2.2) return _room.Type == Room.RoomType.Single ? 190_000 : 240_000;
            if (stayedTime > 1.2) return _room.Type == Room.RoomType.Single ? 150_000 : 190_000;
            if (stayedTime > 0) return _room.Type == Room.RoomType.Single ? 100_000 : 150_000;
            return 0.0;
        }

        private double CalculateDailyFee(DateTime checkInTime)
        {
            var totalDay = 0;
            checkInTime = checkInTime.Hour <= 7 ?
                new DateTime(checkInTime.Year, checkInTime.Month, checkInTime.Day - 1, 12, 0, 0) :
                new DateTime(checkInTime.Year, checkInTime.Month, checkInTime.Day, 12, 0, 0);
            var checkOutTime = DateTime.Now;
            var stayedTime = checkOutTime.Subtract(checkInTime).TotalHours;
            totalDay += (int)stayedTime / 24;
            var totalHours = (int)(stayedTime) % 24;
            if (totalHours > 4.2) return ++totalDay * _room.Rate;
            return totalDay * _room.Rate + CalculateHourlyFee(totalHours);
        }
    }
}