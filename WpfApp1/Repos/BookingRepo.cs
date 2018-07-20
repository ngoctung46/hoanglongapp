using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Model;

namespace WpfApp1.Repos
{
    public class BookingRepo : Base.RepoBase<Booking>
    {
        private static List<Booking> _bookings;

        public BookingRepo()
        {
            Task.Run(async () =>
            {
                var bookings = await All();
                _bookings = bookings.ToList();
            }).Wait();
        }

        public async Task<String> Add(Booking booking)
        {
            var id = await PostAsync(booking);
            booking.Id = id;
            _bookings.Add(booking);
            return id;
        }

        public async Task<IEnumerable<Booking>> All()
        {
            var bookings = await GetAsync();
            return bookings;
        }

        public IEnumerable<Booking> FindByDate(DateTime date)
        {
            return _bookings.Where(x => x.CheckInDate.Equals(date));
        }

        public async Task Remove(string id)
        {
            await DeleteAsync(id);
            _bookings.RemoveAll(x => x.Id == id);
        }

        public IEnumerable<Booking> GetAll() => _bookings;
    }
}