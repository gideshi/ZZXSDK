using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZZX.Entities
{
    [JsonObject]
    public class CustomerRefund
    {
        [JsonProperty("loanId")]
        public string LoanId { get; set; }
        [JsonProperty("refundCapital")]
        public double RefundCapital { get; set; }
        [JsonProperty("refundInterest")]
        public double RefundInterest { get; set; }
        [JsonProperty("refundCommission")]
        public double RefundCommission { get; set; }
        [JsonProperty("amount")]
        public double Amount { get; set; }
        [JsonProperty("periodNumber")]
        public int PeriodNumber { get; set; }
        [JsonProperty("refundDefaultInterest")]
        public double RefundDefaultInterest { get; set; }
        [JsonProperty("overdueDays")]
        public int OverdueDays { get; set; }
    }
}
