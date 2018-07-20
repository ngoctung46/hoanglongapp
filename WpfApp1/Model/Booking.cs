using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Model.Base;

namespace WpfApp1.Model
{
    public class Booking : ModelBase
    {
        [JsonProperty("customerName")]
        public string CustomerName { get; set; }

        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonProperty("checkInDate")]
        public DateTime CheckInDate { get; set; }

        [JsonProperty("roomType")]
        public string RoomType { get; set; }

        [JsonProperty("note")]
        public string Note { get; set; }
    }
}