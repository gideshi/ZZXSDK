using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZZX.Response;

namespace ZZX.Request
{
    public class LoanApplyRequest : IZZXRequest<LoanApplyResponse>
    {
        public string reqparams { get; set; }

        public string GetApiName()
        {
            return Api.loanApply;
        }

        public IDictionary<string, string> GetParameters()
        {
            ZZXDictionary parameters = new ZZXDictionary();
            parameters.Add("params", this.reqparams);
            return parameters;
        }
    }
}
