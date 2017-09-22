using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ZZX;
using ZZX.Request;
using ZZX.Response;
using ZZX.Util;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Generators;
using System.IO;
using Newtonsoft.Json.Linq;
using ZZX.Entities;

namespace ZZXTest
{
    class Program
    {
        static void Main(string[] args)
        {
            jsontest();
            singtest();
            Console.ReadLine();
        }


        static void singtest()
        {
            string privateKey = Cfg.Get("privateKey");
            string publicKey = Cfg.Get("publicKey");
            string myPublicKey = Cfg.Get("myPublicKey");
            string charset = "UTF-8";

            var text = "channelId=\"3\"&method=\"loanApply\"&signType=\"RSA2\"&ver=\"1.0\"";
            var sss = "MWVz2KbCBkKbdMgT2LaEOs7IYrDhG9h+43tkp7oZe9h4cznRaNpHLoKgR6zB+dAt2vaJtnySO1qchj2FNduv0XZV2G26NB+rtAbA4TnesdkQUWYCNJNI3usHsPSRdk68etcjyW66RaF/R/KtXvc9V7xT+HR0BzlA76iaJ88QHtU=";
            //"oOzbzpanqMkEqib40YcnDaw7eb296ORiEE37Ysz/XpJVeJtvqAsZ5yIseXFMsXRjJZ1yCyknuspZ5qoglIDhHkgPn/S2UBnR1f/JuyCHifxW7tJgu1CpbFdHZ7BFHwGmxb97Jx0pOYKaVKW14bTZgnLKepBStT4SjhFeX7LUPoE="
            var t = RSAUtil.Verify(text, sss, publicKey, charset);
            Console.WriteLine($"对方验签：{t}");
        }

        static void jsontest()
        {
            ZZXApiRequest request = new ZZXApiRequest();
            request.Method = "methodtest";
            request.ChannelId = "3";
            request.Sign = "12246534684+12354";
            request.SignType = "RSA2";
            request.Ver = "v1.0";
            request.Params = new
            {
                id = 1,
                name = 2,
                list = new List<Order>()
                {
                    new Order(){ OrderId="1",Name="LA1" },
                    new Order(){ OrderId="2",Name="LA2" },
                    new Order(){ OrderId="3",Name="LA3" }
                }
            };

            var jSetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

            var json_request = JsonConvert.SerializeObject(request, Formatting.None, jSetting);
            var buildq_request = WebUtils.BuildQueryT(JObject.FromObject(request), "UTF-8");

            Console.WriteLine($"json_request:{json_request}");
            Console.WriteLine($"buildq_request:{buildq_request}");

            ZZXApiResponse response = new ZZXApiResponse();
            response.StatusCode = 200;
            response.ErrMsg = "成功";
            response.ChannelId = "3";
            response.Method = "responsemethod";
            response.Parms = new { id = 2, name = "response" };
            response.Sign = "325961321562.55";
            response.SignType = "RSA2";
            response.Ver = "v1.0";

            var json_response = JsonConvert.SerializeObject(response, Formatting.None, jSetting);
            var build_response = WebUtils.BuildQueryT(JObject.FromObject(response), "UTF-8");
            Console.WriteLine($"json_response:{json_response}");
            Console.WriteLine($"build_response:{build_response}");
        }
    }
}
