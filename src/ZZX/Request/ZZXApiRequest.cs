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

        public object Params { get; set; }

        public string Ver { get; set; }

        public string GetApiName()
        {
            return Method;
        }

        public object GetParams()
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

        public IDictionary<string, object> GetParameters()
        {
            ZZXDictionary parameters = new ZZXDictionary();
            parameters.Add("params", Params);
            return parameters;
        }


    }
}
