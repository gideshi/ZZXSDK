using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZZX.Response
{
    [JsonObject]
    public class ZZXApiResponse : ZZXResponse
    {
        public int MyProperty { get; set; }
    }
}
