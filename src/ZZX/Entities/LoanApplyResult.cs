using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZZX.Entities
{
    [JsonObject]
    public class LoanApplyResult
    {
        [JsonProperty("loanId")]
        public string LoanId { get; set; }
        [JsonProperty("result")]
        public int Result { get; set; }
        [JsonProperty("commissions")]
        public double Commissions { get; set; }
        [JsonProperty("reason")]
        public string Reason { get; set; }
        [JsonProperty("loanAmount")]
        public double LoanAmount { get; set; }
        [JsonProperty("loanTerm")]
        public int LoanTerm { get; set; }
        [JsonProperty("paymentOption")]
        public int PaymentOption { get; set; }
        [JsonProperty("orders")]
        public List<ResultOrder> Orders { get; set; }
    }

    [JsonObject]
    public class ResultOrder
    {
        [JsonProperty("sourceOrderId")]
        public string SourceOrderId { get; set; }
        [JsonProperty("loanAmount")]
        public double LoanAmount { get; set; }
    }
}
