using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZZX.Entities
{
    [JsonObject]
    public class Request
    {
        [JsonProperty("method")]
        public string Method { get; set; }
        [JsonProperty("ver")]
        public string Ver { get; set; }
        [JsonProperty("channelId")]
        public string ChannelId { get; set; }
        [JsonProperty("signType")]
        public string SignType { get; set; }
        [JsonProperty("sign")]
        public string Sign { get; set; }
        [JsonProperty("params")]
        public object Parms { get; set; }
    }
}
