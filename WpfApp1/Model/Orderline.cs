using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WpfApp1.Model.Base;
using WpfApp1.Repos;

namespace WpfApp1.Model
{
    public class Orderline : ModelBase
    {
        [JsonProperty("serviceId")]
        public string ServiceId { get; set; }

        [JsonProperty("orderId")]
        public string OrderId { get; set; }

        [JsonProperty("quantity")]
        public double Quantity { get; set; }

        [JsonProperty("price")]
        public double Price { get; set; }

        [JsonProperty("serviceName")]
        public string ServiceName { get; set; }

        [JsonProperty("total")]
        public double Total { get; set; }

        [JsonIgnore]
        public Service Service { get; set; }
    }
}