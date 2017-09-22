using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZZX
{
    [JsonObject]
    public interface IZZXRequest<T> where T : ZZXResponse
    {
        [JsonProperty("method")]
        string Method { get; set; }
        [JsonProperty("ver")]
        string Ver { get; set; }
        [JsonProperty("channelId")]
        string ChannelId { get; set; }
        [JsonProperty("signType")]
        string SignType { get; set; }
        [JsonProperty("sign")]
        string Sign { get; set; }
        [JsonProperty("params")]
        object Params { get; set; }


        IDictionary<string, object> GetParameters();
    }
}
