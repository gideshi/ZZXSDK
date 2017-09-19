using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZZX.Entities
{
    [JsonObject]
    public class RefundNotify
    {
        [JsonProperty("refundType")]
        public int RefundType { get; set; }
        [JsonProperty("loanId")]
        public string LoanId { get; set; }
        [JsonProperty("amount")]
        public double Amount { get; set; }
        [JsonProperty("periodNumber")]
        public int ReriodNumber { get; set; }
    }
}
