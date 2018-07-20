using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Model.Base;

namespace WpfApp1.Model
{
    public class Room : ModelBase
    {
        public enum RoomStatus
        {
            Occupied,
            Available,
            Dirty,
            Clean,
            Broken,
            CustomerOut,
            Booked
        }

        public enum RoomType
        {
            Single, Double
        }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("rate")]
        public double Rate { get; set; }

        [JsonProperty("status")]
        public RoomStatus Status { get; set; }

        [JsonProperty("type")]
        public RoomType Type { get; set; }

        [JsonProperty("customerId")]
        public string CustomerId { get; set; }

        [JsonProperty("orderId")]
        public string OrderId { get; set; }
    }
}