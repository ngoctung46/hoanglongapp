using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using WpfApp1.Model;
using System.Reactive;
using System.Reactive.Linq;
using WpfApp1.Helper;
using WpfApp1.Repos;
using WpfApp1.ViewModel.CustomerViewModels;
using WpfApp1.ViewModel.HomeViewModels;

namespace WpfApp1.ViewModel.CustomerViewModels
{
    public class CustomerControlViewModel : ReactiveObject
    {
        private string _identity;
        private readonly ObservableAsPropertyHelper<bool> _isNewCustomer;
        private readonly CustomerRepo _customerRepo;
        private readonly RoomRepo _roomRepo;
        private readonly OrderRepo _orderRepo;
        private readonly OrderlineRepo _orderLineRepo;
        private readonly ServiceRepo _serviceRepo;
        private readonly Room _room;
        private readonly RoomControlViewModel _roomControlViewModel;
        private DateTime _checkInTime = DateTime.Now;
        private DateTime? _checkOutTime;

        private Customer _customer;

        public bool IsNewCustomer => _isNewCustomer.Value;
        public List<Customer> Customers;

        public Customer Customer
        {
            get => _customer;
            set => this.RaiseAndSetIfChanged(ref _customer, value);
        }

        public string Identity
        {
            get => _identity;
            set => this.RaiseAndSetIfChanged(ref _identity, value);
        }

        public DateTime CheckInTime
        {
            get => _checkInTime;
            set => this.RaiseAndSetIfChanged(ref _checkInTime, value);
        }

        public DateTime? CheckOutTime
        {
            get => _checkOutTime;
            set => this.RaiseAndSetIfChanged(ref _checkOutTime, value);
        }

        public ReactiveList<string> Provinces;
        public ReactiveList<string> Nations;
        public ReactiveCommand<Unit, Task<bool>> AcceptCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> SearchCustomerCommand { get; protected set; }

        public CustomerControlViewModel(RoomControlViewModel roomControlViewModel)
        {
            _roomControlViewModel = roomControlViewModel;
            _room = _roomControlViewModel.Room;
            _isNewCustomer = this.WhenAnyValue(x => x.Customer)
                .Select(x => x?.Id == null)
                .ToProperty(this, x => x.IsNewCustomer);
            _customerRepo = new CustomerRepo();
            _roomRepo = new RoomRepo();
            _orderRepo = new OrderRepo();
            _orderLineRepo = new OrderlineRepo();
            _serviceRepo = new ServiceRepo();
            Customers = _customerRepo.GetAll().ToList();
            AcceptCommand = ReactiveCommand.Create(Accept);
            SearchCustomerCommand = ReactiveCommand.Create(SearchCustomer);
            Provinces = new ReactiveList<string>(Utility.GetProvinces());
            Nations = new ReactiveList<string>(Utility.GetNations());
            this.WhenAnyValue(x => x.CheckInTime)
                .Select(x =>
                {
                    if (Customer == null) return DateTime.Now;
                    var checkInDate = Customer.CheckInDate;
                    return new DateTime(checkInDate.Year, checkInDate.Month, checkInDate.Day, x.Hour, x.Minute, x.Second).ToUniversalTime();
                }).BindTo(this, x => x.Customer.CheckInDate);
            this.WhenAnyValue(x => x.CheckOutTime)
                .Select(x =>
                {
                    if (Customer == null || Customer?.CheckOutDate == null) return new DateTime();
                    var checkOutDate = Customer.CheckOutDate.Value;
                    return new DateTime(checkOutDate.Year, checkOutDate.Month, checkOutDate.Day, x.Value.Hour, x.Value.Minute, x.Value.Second).ToUniversalTime();
                }).BindTo(this, x => x.Customer.CheckOutDate);
        }

        private void SearchCustomer()
        {
            Customer = _customerRepo.FindByIdentity(Identity) ?? new Customer { Identity = Identity };
        }

        private async Task<bool> Accept()
        {
            if (Customer.CheckOutDate.Equals(new DateTime())) Customer.CheckOutDate = null;
            if (Customer.Id == null) Customer.Id = await _customerRepo.Add(Customer);
            else await _customerRepo.Update(Customer);
            _room.Status = Room.RoomStatus.Occupied;
            _room.CustomerId = Customer.Id;
            _roomControlViewModel.Status = Room.RoomStatus.Occupied;
            _roomControlViewModel.Customer = Customer;
            _roomControlViewModel.Room.CustomerId = Customer.Id;
            var newOrder = Utility.CreateNewOrder(Customer.Id, _room, Customer.CheckInDate);
            var orderId = await _orderRepo.Add(newOrder);
            // var service = _serviceRepo.GetRoomRateService();
            // var newOrderLine = Utility.CreateNewOrderline(service.Id, orderId, 1);
            // await _orderLineRepo.Add(newOrderLine);
            _room.OrderId = orderId;
            return await _roomRepo.Update(_room);
        }
    }
}