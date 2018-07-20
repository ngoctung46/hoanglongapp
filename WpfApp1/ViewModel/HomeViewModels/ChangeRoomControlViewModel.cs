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

namespace WpfApp1.ViewModel.HomeViewModels
{
    public class ChangeRoomControlViewModel : ReactiveObject
    {
        private readonly RoomRepo _roomRepo;
        private readonly OrderRepo _orderRepo;
        private Room _room;
        private readonly Room _oldRoom;

        public Room Room
        {
            get => _room;
            set => this.RaiseAndSetIfChanged(ref _room, value);
        }

        public ReactiveList<Room> Rooms;

        public ReactiveCommand<Unit, Unit> AcceptCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; protected set; }

        public ChangeRoomControlViewModel(Room room)
        {
            _oldRoom = room;
            _roomRepo = new RoomRepo();
            _orderRepo = new OrderRepo();
            var rooms = _roomRepo.AvailableRooms;
            Rooms = new ReactiveList<Room>(rooms);
            Room = Rooms[0];
            AcceptCommand = ReactiveCommand.CreateFromTask(Accept);
            CancelCommand = ReactiveCommand.Create(Cancel);
        }

        private async Task Accept()
        {
            Room.OrderId = _oldRoom.OrderId;
            Room.CustomerId = _oldRoom.CustomerId;
            Room.Status = Room.RoomStatus.Occupied;
            _oldRoom.OrderId = String.Empty;
            _oldRoom.CustomerId = String.Empty;
            _oldRoom.Status = Room.RoomStatus.Dirty;
            var order = new Order() { RoomId = Room.Id };
            var success = await _roomRepo.Update(_oldRoom);
            if (success) await _roomRepo.Update(Room);
            if (success) await _orderRepo.Update(order);
        }

        private void Cancel()
        {
        }
    }
}