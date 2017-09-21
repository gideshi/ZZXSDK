using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZZX.Response;

namespace ZZX.Request
{
    [JsonObject]
    public class ZZXApiRequest : IZZXRequest<ZZXApiResponse>
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
        public JObject Parms { get; set; }
    }
}
