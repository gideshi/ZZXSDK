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

namespace ZZXTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //signtest();
            SignTest();
            Console.ReadLine();
        }

        static void signtest()
        {
            string privateKey = Cfg.Get("privateKey");
            string publicKey = Cfg.Get("publicKey");
            string myPublicKey = Cfg.Get("myPublicKey");
            string charset = "UTF-8";
            var text = "123";
            var sss = "mLk38/34Cv/O2514rMkK4fTsmaSD04YhWqM7cWp6Cxq9Vi6t4jySYIOglmRvW09bq9+UcmNCMPgVCrq0Zu06a9Mw7EXq3eHcULQTjL3QNJGzkjq27nnl2axsZRpg47vJSc9h5YGgCsFQeO+gVYDNN/0QX79PpdudyytB7kHVFtk=";
            //var s = RSAUtil.Encrypt(text, privateKey, charset);

            var s = RSAUtil.Sign(text, privateKey, charset);

            Console.WriteLine($"签名：{s}");

            //用我的公钥来验下这个签

            var tt = RSAUtil.Verify(text, s, myPublicKey, charset);
            Console.WriteLine($"验签：{tt}");

            var t = RSAUtil.Verify(text, sss, publicKey, charset);
            Console.WriteLine($"对方验签：{t}");
        }

        static void defaulttest()
        {
            //var unsign ="channelId=2&method=getLoanDetailInfo&params={\"loanDate\":\"2016 - 12 - 09\",\"commissions\":60,\"loanAmount\":2000,\"balance\":2080.53,\"refunds\":[{\"periodNumber\":1,\"dueDate\":\"20170108\",\"dueAmount\":693.51,\"status\":3},{\"periodNumber\":2,\"dueDate\":\"20170207\",\"dueAmount\":693.51,\"status\":3},{\"periodNumber\":3,\"dueDate\":\"20170309\",\"dueAmount\":693.51,\"status\":3}]}&signType=RSA&ver=1.0";



            //var sign = RSAUtil.Sign(unsign,Cfg.Get("privateKey"), "UTF-8");

            //Console.WriteLine(sign);

            string ss = "";
            string url = Cfg.Get("url");
            string channelId = Cfg.Get("channelId");
            string privateKey = Cfg.Get("privateKey");
            string publicKey = Cfg.Get("publicKey");
            string charset = "UTF-8";

            //ZZXClient client = new ZZXClient(url, channelId, privateKey, publicKey, charset);

            ////LoanApplyRequest request = new LoanApplyRequest();

            //////request //设置请求参数
            ////LoanApplyResponse response = client.Execute(request);
            //ZZXApiRequest request = new ZZXApiRequest();
            //request.Method = "loanApply";//设置接口
            //request.Params = "{\"loanApplySubmit \":\"123456\"}";
            //ZZXResponse response = client.Execute(request);

            //Console.WriteLine(response);
            //SignTest();
            //string publicKey = Cfg.Get("publicKey");
            //string privateKey = Cfg.Get("privateKey");
            //string charset = "UTF-8";
            //var ttt = RSAUtil.Sign("123", privateKey, charset);
            //Console.WriteLine(ttt);
            //var sss = "mLk38/34Cv/O2514rMkK4fTsmaSD04YhWqM7cWp6Cxq9Vi6t4jySYIOglmRvW09bq9+UcmNCMPgVCrq0Zu06a9Mw7EXq3eHcULQTjL3QNJGzkjq27nnl2axsZRpg47vJSc9h5YGgCsFQeO+gVYDNN/0QX79PpdudyytB7kHVFtk=";
            //var tt=RSAUtil.Decrypt(sss, publicKey, charset);
            //Console.WriteLine(tt);

            //string decryptedResponse = RSAUtil.Decrypt(sss, privateKey, charset);
            //bool success = RSAUtil.Verify(decryptedResponse, sss, publicKey, charset);
            //Console.WriteLine(success);
            //ZZXClient client = new ZZXClient(url, channelId, privateKey, publicKey, charset);
            //var t = client.decryptAndVerifySign("MTIz", sss);
            //Console.WriteLine(t);
        }

        static void SignTest()
        {
           
            string ss = "";
            string url = Cfg.Get("url");
            string channelId = Cfg.Get("channelId");
            string privateKey = Cfg.Get("privateKey");
            string publicKey = Cfg.Get("publicKey");
            string charset = "UTF-8";

            ZZXClient zzxclient = new ZZXClient(url, channelId, privateKey, publicKey, charset);
            ZZXApiRequest request = new ZZXApiRequest();
            request.Method = "loanApply ";//设置接口
            var tt = new { id="123"};
            request.Params = JsonConvert.SerializeObject(tt);
            //ZZXResponse response = client.Execute(request);
            var dic = zzxclient.getSystemParams(request);//参数
            //var parms = WebUtils.BuildQuery(dic, charset);//组装成参数
            ////直接序列化
            ////var parms=
            var sign = dic["sign"].ToString();
            //dic.Add("sign", sign);
            //JsonConvert.SerializeObject(dic);
            //var d = dic.OrderBy(p => p.Key).ToDictionary(p => p.Key, o => o.Value);

            //移除sign 试试
            //dic.Remove("sign");

            var parms = JsonConvert.SerializeObject(dic);

            //using (HttpClient client = new HttpClient())
            //{
            //    IEnumerable<KeyValuePair<string, string>> queryPart = new List<KeyValuePair<string, string>>(){
            //                        new KeyValuePair<string, string>("params",parms),
            //                        new KeyValuePair<string, string>("sign",sign)
            //                    };
            //    HttpContent q = new FormUrlEncodedContent(queryPart);
            //    //url = baseurl + url;

            //    using (HttpResponseMessage response = client.PostAsync("https://ssl-scf.xingyoucai.com/api/v1/antai/checkSign.do", q).Result)
            //    {
            //        using (HttpContent content = response.Content)
            //        {
            //            var html = content.ReadAsStringAsync().Result;
            //            Console.WriteLine(html);
            //        }
            //    }

            //}

            HttpClient client = new HttpClient();
            var t = client.GetAsync("https://ssl-scf.xingyoucai.com/api/v1/antai/checkSign.do?params=" + parms + "&sign=" + sign).Result;



            //var body = JsonConvert.SerializeObject(dic);

            //var result = RequestHelper.HttpPost("https://ssl-scf.xingyoucai.com/api/v1/antai/checkSign.do", body: body);

            Console.WriteLine(t);

        }
    }
}
