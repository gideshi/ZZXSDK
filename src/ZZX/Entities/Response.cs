using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZZX.Entities
{
    [JsonObject]
    public class Response
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
