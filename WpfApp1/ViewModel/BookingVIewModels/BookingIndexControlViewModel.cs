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

namespace WpfApp1.ViewModel.BookingVIewModels
{
    public class BookingIndexControlViewModel : ReactiveObject
    {
        private readonly BookingRepo _bookingRepo;

        private string _customerName;

        private readonly Interaction<Unit, string> filePath;

        public Interaction<Unit, string> FilePath => filePath;

        public string CustomerName
        {
            get => _customerName;
            set => this.RaiseAndSetIfChanged(ref _customerName, value);
        }

        private string _phoneNumber;

        public string PhoneNumber
        {
            get => _phoneNumber;
            set => this.RaiseAndSetIfChanged(ref _phoneNumber, value);
        }

        private DateTime _checkInDate = DateTime.Now;

        public DateTime CheckInDate
        {
            get => _checkInDate;
            set => this.RaiseAndSetIfChanged(ref _checkInDate, value);
        }

        private DateTime _checkInTime = DateTime.Now;

        public DateTime CheckInTime
        {
            get => _checkInTime;
            set => this.RaiseAndSetIfChanged(ref _checkInTime, value);
        }

        private string _roomType = "Phòng Đơn";

        public string RoomType
        {
            get => _roomType;
            set => this.RaiseAndSetIfChanged(ref _roomType, value);
        }

        private string _note;

        public string Note
        {
            get => _note;
            set => this.RaiseAndSetIfChanged(ref _note, value);
        }

        private Booking _booking;

        public Booking Booking
        {
            get => _booking;
            set => this.RaiseAndSetIfChanged(ref _booking, value);
        }

        public ReactiveList<Booking> Bookings { get; set; }
        public ReactiveCommand<Unit, Unit> SaveCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> DeleteCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> ExportCommand { get; protected set; }

        public BookingIndexControlViewModel()
        {
            _bookingRepo = new BookingRepo();
            filePath = new Interaction<Unit, string>();
            SetDefault();
            var canExport = this.WhenAnyValue(x => x.Bookings.Count, count => count > 0);
            SaveCommand = ReactiveCommand.CreateFromTask(Save);
            DeleteCommand = ReactiveCommand.CreateFromTask(Delete);
            ExportCommand = ReactiveCommand.CreateFromTask(Export, canExport);
        }

        private void SetDefault()
        {
            var bookings = _bookingRepo.GetAll();
            Bookings = new ReactiveList<Booking>(bookings) { ChangeTrackingEnabled = true };
            Booking = Bookings.Count > 0 ? Bookings[0] : new Booking();
        }

        private async Task Save()
        {
            var checkInDate = new DateTime(CheckInDate.Year, CheckInDate.Month, CheckInDate.Day, CheckInTime.Hour, CheckInTime.Minute, CheckInTime.Second);
            Booking = new Booking
            {
                CustomerName = CustomerName,
                PhoneNumber = PhoneNumber,
                CheckInDate = checkInDate,
                RoomType = RoomType,
                Note = Note
            };

            if (Booking == null) return;
            if (Booking.Id != null) return;
            Booking.Id = await _bookingRepo.Add(Booking);
            Bookings.Add(Booking);
            ResetData();
        }

        private async Task Delete()
        {
            if (Booking?.Id == null) return;
            await _bookingRepo.Remove(Booking.Id);
            Bookings.Remove(Booking);
        }

        private void ResetData()
        {
            CustomerName = String.Empty;
            PhoneNumber = String.Empty;
            CheckInDate = DateTime.Now;
            CheckInTime = DateTime.Now;
            Note = String.Empty;
            RoomType = "Phòng Đơn";
        }

        private async Task Export()
        {
            try
            {
                var filePath = await this.filePath.Handle(Unit.Default);
                Utility.ExportBookings(filePath, Bookings.ToList());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
        }
    }
}