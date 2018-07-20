using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Controls.CustomerControls;
using WpfApp1.Interfaces;
using WpfApp1.Model;
using WpfApp1.Repos;

namespace WpfApp1.ViewModel.HomeViewModels
{
    public class RoomControlViewModel : ReactiveObject
    {
        private Room _room;

        private Room.RoomStatus _status;

        private Customer _customer;

        private readonly ObservableAsPropertyHelper<bool> _isOccupied;

        private readonly CustomerRepo _customerRepo;

        private readonly RoomRepo _roomRepo;

        public ReactiveCommand<Unit, Unit> CheckOutCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> CleaningCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> OpenDialogCommand { get; protected set; }
        public ReactiveCommand<Unit, Customer> ShowCustomerInfoCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> ViewReceiptCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> AddServiceCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> ChangeRoomCommand { get; protected set; }
        public bool IsOccupied => _isOccupied.Value;

        public Customer Customer
        {
            get => _customer;
            set => this.RaiseAndSetIfChanged(ref _customer, value);
        }

        public Room Room
        {
            get => _room;
            set => this.RaiseAndSetIfChanged(ref _room, value);
        }

        public Room.RoomStatus Status
        {
            get => _status;
            set => this.RaiseAndSetIfChanged(ref _status, value);
        }

        public RoomControlViewModel(Room room)
        {
            if (room == null) return;
            _customerRepo = new CustomerRepo();
            _roomRepo = new RoomRepo();
            Room = room;
            Status = Room.Status;
            CheckOutCommand = ReactiveCommand.Create(CheckOut);
            CleaningCommand = ReactiveCommand.Create(Cleaning);
            OpenDialogCommand = ReactiveCommand.Create(OpenDialog);
            ShowCustomerInfoCommand = ReactiveCommand.Create(ShowCustomerInfo);
            ViewReceiptCommand = ReactiveCommand.Create(ViewReceipt);
            AddServiceCommand = ReactiveCommand.Create(AddService);
            ChangeRoomCommand = ReactiveCommand.Create(ChangeRoom);
            _isOccupied = this.WhenAnyValue(x => x.Status, s => s != Room.RoomStatus.Available).ToProperty(this, x => x.IsOccupied);
        }

        private void OpenDialog()
        {
        }

        private void ChangeRoom()
        {
        }

        private async void CheckOut()
        {
            Customer = new Customer();
            Room.Status = Room.RoomStatus.Dirty;
            Status = Room.RoomStatus.Dirty;
            Room.CustomerId = String.Empty;
            await _roomRepo.Update(Room);
        }

        private async void Cleaning()
        {
            var status = Status == Room.RoomStatus.Occupied ? Room.RoomStatus.Dirty : Room.RoomStatus.Occupied;
            Room.Status = status;
            Status = status;
            if (String.IsNullOrEmpty(Room.CustomerId))
            {
                Status = Room.RoomStatus.Available;
                Room.Status = Status;
            }
            await _roomRepo.Update(Room);
        }

        private void ViewReceipt()
        {
        }

        private Customer ShowCustomerInfo()
        {
            var customer = _customerRepo.FindById(Room.CustomerId);
            customer.CheckInDate = customer.CheckInDate.ToLocalTime();
            if (customer.CheckOutDate != null) customer.CheckOutDate = customer.CheckOutDate.Value.ToLocalTime();
            return customer;
        }

        private void AddService()
        {
        }
    }
}