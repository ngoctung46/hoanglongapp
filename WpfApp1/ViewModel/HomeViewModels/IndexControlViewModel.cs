using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfApp1.Model;
using WpfApp1.Repos;

namespace WpfApp1.ViewModel.HomeViewModels
{
    public class IndexControlViewModel : ReactiveObject
    {
        public ReactiveList<Room> Rooms;
        private readonly RoomRepo _roomRepo;

        private int _availlableCount;

        public int AvailableCount
        {
            get => _availlableCount;
            set => this.RaiseAndSetIfChanged(ref _availlableCount, value);
        }

        private int _occupiedCount;

        public int OccupiedCount
        {
            get => _occupiedCount;
            set => this.RaiseAndSetIfChanged(ref _occupiedCount, value);
        }

        public ReactiveCommand<Unit, bool> InitializeCommand { get; protected set; }

        public IndexControlViewModel()
        {
            _roomRepo = new RoomRepo();
            InitializeCommand = ReactiveCommand.CreateFromTask(InitializeRooms);
            this.WhenAnyValue(x => x.AvailableCount).Select(x => Rooms?.Count - x).BindTo(this, x => x.OccupiedCount);
        }

        private async Task<bool> InitializeRooms()
        {
            var app = ((App)Application.Current);
            if (app.Rooms != null) return false;
            var rooms = await _roomRepo.All();
            Rooms = new ReactiveList<Room>(rooms);
            app.Rooms = Rooms;
            AvailableCount = Rooms.Count(x => x.Status == Room.RoomStatus.Available);
            return true;
        }
    }
}