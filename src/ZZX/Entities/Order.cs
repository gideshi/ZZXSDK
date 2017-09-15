using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZZX.Entities
{
    [JsonObject]
    public class Order
    {
        [JsonProperty("orderId")]
        public string OrderId { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("organizationId")]
        public string OrganizationId { get; set; }
        [JsonProperty("organization")]
        public string Organization { get; set; }
        [JsonProperty("mobile")]
        public string Mobile { get; set; }
        [JsonProperty("cardno")]
        public string CardNO { get; set; }
        [JsonProperty("divideRate")]
        public float DivideRate { get; set; }
        [JsonProperty("level")]
        public string Level { get; set; }
        [JsonProperty("orderDate")]
        public string OrderDate { get; set; }
        [JsonProperty("packageDuration")]
        public string PackageDuration { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
