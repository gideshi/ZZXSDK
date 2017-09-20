using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZZX.Entities
{
    [JsonObject]
    public class Payment
    {
        [JsonProperty("loanId")]
        public string LoanId { get; set; }

        [JsonProperty("date")]
        public string Date { get; set; }
        [JsonProperty("amount")]
        public double Amount { get; set; }
        [JsonProperty("account")]
        public List<Account> Accounts { get; set; }
        [JsonProperty("refunds")]
        public List<Refund> Refunds { get; set; }
        
    }
    [JsonObject]
    public class Account
    {
        [JsonProperty("accountName")]
        public string AccountName { get; set; }
        [JsonProperty("bankCard")]
        public string BankCard { get; set; }
        [JsonProperty("bankBranch")]
        public string BankBranch { get; set; }
    }

    [JsonObject]
    public class Refund
    {
        [JsonProperty("periodNumber")]
        public int PeriodNumber { get; set; }
        [JsonProperty("dueDate")]
        public string DueDate { get; set; }
        [JsonProperty("dueCapital")]
        public double DueCapital { get; set; }
        [JsonProperty("dueInterest")]
        public double DueInterest { get; set; }
        [JsonProperty("dueAmount")]
        public double DueAmount { get; set; }
    }
}
