using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ZZX
{
    [JsonObject]
    public abstract class ZZXResponse
    {
        [JsonProperty("statusCode")]
        public int StatusCode { get; set; }
        [JsonProperty("errMsg")]
        public string ErrMsg { get; set; }
        [JsonProperty("channelId")]
        public string ChannelId { get; set; }
        [JsonProperty("method")]
        public string Method { get; set; }
        [JsonProperty("params")]
        public object Parms { get; set; }
        [JsonProperty("sign")]
        public string Sign { get; set; }
        [JsonProperty("signType")]
        public string SignType { get; set; }
        [JsonProperty("ver")]
        public string Ver { get; set; }
    }
}
