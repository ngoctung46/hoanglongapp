using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WpfApp1.Model.Base;

namespace WpfApp1.Model
{
    public class Service : ModelBase
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("unitInStock")]
        public double UnitInStock { get; set; }

        [JsonProperty("sold")]
        public double Sold { get; set; }

        [JsonProperty("currentPrice")]
        public double CurrentPrice { get; set; }

        [JsonProperty("costPrice")]
        public double CostPrice { get; set; }

        [JsonProperty("isRoomRate")]
        public bool IsRoomRate { get; set; }
    }
}