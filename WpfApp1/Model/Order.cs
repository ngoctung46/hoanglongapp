using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WpfApp1.Model.Base;

namespace WpfApp1.Model
{
    public class Order : ModelBase
    {
        [JsonProperty("orderLineIds")]
        public List<String> OrderLineIds { get; set; }

        [JsonProperty("customerId")]
        public string CustomerId { get; set; }

        [JsonProperty("roomId")]
        public string RoomId { get; set; }

        [JsonProperty("roomName")]
        public string RoomName { get; set; }

        [JsonProperty("checkInTime")]
        public DateTime CheckInTime { get; set; }

        [JsonProperty("checkOutTime")]
        public DateTime CheckOutTime { get; set; }

        [JsonProperty("discount")]
        public double Discount { get; set; }

        [JsonProperty("adjustment")]
        public double Adjustment { get; set; }

        [JsonProperty("total")]
        public double Total { get; set; }

        [JsonIgnore]
        public List<Orderline> OrderLines { get; set; }

        [JsonIgnore]
        public Room Room { get; set; }

        public Order()
        {
            Room = new Room();
            OrderLines = new List<Orderline>();
        }
    }
}