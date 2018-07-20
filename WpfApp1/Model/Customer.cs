using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Model.Base;

namespace WpfApp1.Model
{
    public class Customer : ModelBase
    {
        [JsonProperty("identity")]
        public string Identity { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("birthDate")]
        public DateTime BirthDate { get; set; } = DateTime.Now;

        [JsonProperty("birthPlace")]
        public string BirthPlace { get; set; }

        [JsonProperty("issueDate")]
        public DateTime IssueDate { get; set; } = DateTime.Now;

        [JsonProperty("expiryDate")]
        public DateTime ExpiryDate { get; set; } = DateTime.Now;

        [JsonProperty("issuePlace")]
        public string IssuePlace { get; set; }

        [JsonProperty("addressLine1")]
        public string AddressLine1 { get; set; }

        [JsonProperty("addressLine2")]
        public string AddressLine2 { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("checkInDate")]
        public DateTime CheckInDate { get; set; } = DateTime.UtcNow;

        [JsonProperty("checkOutDate")]
        public DateTime? CheckOutDate { get; set; }

        [JsonProperty("orderId")]
        public string OrderId { get; set; }

        [JsonIgnore]
        public string AddressDisplay
            => $"{AddressLine1}{Environment.NewLine}" +
               $"{AddressLine2}{Environment.NewLine}" +
               $"{City}{Environment.NewLine}" +
               $"{Country}";

        [JsonIgnore]
        public string IdentityDisplay
            => $"{Identity}{Environment.NewLine}" +
               $"Ngày Cấp : {IssueDate:dd/MM/yyyy}{Environment.NewLine}" +
               $"Ngày Hết Hạn: {ExpiryDate:dd/MM/yyyy}{Environment.NewLine}" +
               $"Nơi Cấp: {IssuePlace}";

        [JsonIgnore]
        public string CheckInDateDisplay => $"{CheckInDate:dd/MM/yyyy HH:mm:ss}";

        [JsonIgnore]
        public string CheckOutDateDisplay => $"{CheckOutDate:dd/MM/yyyy HH:mm:ss}";

        [JsonIgnore]
        public string BirthDateDisplay => $"{BirthDate:dd/MM/yyyy}";

        [JsonIgnore]
        public string RoomName { get; set; }
    }
}