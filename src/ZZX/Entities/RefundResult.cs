using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZZX.Entities
{
    [JsonObject]
    public class RefundResult
    {
        [JsonProperty("result")]
        public int Result { get; set; }
        [JsonProperty("reason")]
        public string Reason { get; set; }
        [JsonProperty("date")]
        public string Date { get; set; }
        [JsonProperty("periodNumber")]
        public int PeriodNumber { get; set; }
        [JsonProperty("loanId")]
        public string LoanId { get; set; }
        [JsonProperty("amount")]
        public double Amount { get; set; }
        [JsonProperty("refundCapital")]
        public double RefundCapital { get; set; }
        [JsonProperty("refundInterest")]
        public double RefundInterest { get; set; }
        [JsonProperty("refundCommission")]
        public double RefundCommission { get; set; }
        [JsonProperty("refundDefaultInterest")]
        public double RefundDefaultInterest { get; set; }
        [JsonProperty("refundFlag")]
        public int RefundFlag { get; set; }
        [JsonProperty("refundType")]
        public int RefundType { get; set; }
    }
}
