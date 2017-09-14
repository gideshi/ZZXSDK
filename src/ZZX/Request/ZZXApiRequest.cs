using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZZX.Response;

namespace ZZX.Request
{
    public class ZZXApiRequest : IZZXRequest<ZZXApiResponse>
    {
        public string Method { get; set; }

        public string Params { get; set; }

        public string Ver { get; set; }

        public string GetApiName()
        {
            return Method;
        }

        public string GetParams()
        {
            return Params;
        }

        public void SetApiVersion(string apiVersion)
        {
            Ver = apiVersion;
        }

        public string GetApiVersion()
        {
            return Ver;
        }

        public IDictionary<string, string> GetParameters()
        {
            ZZXDictionary parameters = new ZZXDictionary();
            parameters.Add("params", Params);
            return parameters;
        }


    }
}
