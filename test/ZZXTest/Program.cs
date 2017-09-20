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
            //signtest();
            //SignTest();
            //signzzxtest();
            //Duqutupian();
            //jsontest();
            //webtuiltest();
            generatorsign();
            Console.ReadLine();
        }

        static void webtuiltest()
        {
            Response p = new Response()
            {
                //StatusCode = 200,
                //ErrMsg = "success",
                Method = "loanApplyResultNotify",
                Ver = "1.0",
                ChannelId = "3",
                SignType = "RSA2",
                //Sign = "1514564613515156",
                Parms = new { id = 1, name = "LA" }
            }; 
             var jSetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            var json = JsonConvert.SerializeObject(p, Formatting.Indented, jSetting);
            var t = WebUtils.BuildQueryT((JObject)JsonConvert.DeserializeObject(json), "utf-8");

            Console.WriteLine(t);
        }

        static void jsontest()
        {
            string str = "{\"3\":123,body:456,list:{title:'abc',body:'what'}}";
            JObject jo = JObject.Parse(str);
            if (jo.Property("3") == null || jo.Property("3").ToString() == "")
            {
                Console.WriteLine("键值key不存在！");
            }
            bool hasErr = jo.Properties().Any(p => p.Name == "err");//或是这样
            IEnumerable<JProperty> properties = jo.Properties();
            foreach (JProperty item in properties)
            {
                Console.WriteLine(item.Name + ":" + item.Value);
            }
        }

        static void Duqutupian()
        {
            var path = "F:\\test.jpg";
            FileStream fs = File.OpenRead(path); //OpenRead
            int filelength = 0;
            filelength = (int)fs.Length; //获得文件长度 
            Byte[] bytes = new Byte[filelength]; //建立一个字节数组 
            fs.Read(bytes, 0, filelength); //按字节流读取 

            string base64 = Convert.ToBase64String(bytes);

            Console.WriteLine(base64);
        }

        static void signzzxtest()
        {
            string privateKey = Cfg.Get("privateKey");
            string publicKey = Cfg.Get("publicKey");
            string myPublicKey = Cfg.Get("myPublicKey");
            string charset = "UTF-8";

            var dic = new Dictionary<string, object>();
            dic["channelId"] = "3";
            dic["method"] = "loanApply";
            //dic["params"] = new { loanId = "20170915174747000008" };
            dic["params"] = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(new { loanId = "20170915174747000008" }));
            dic["signType"] = "RSA2";
            dic["ver"] = "1.0";
            //dic["statusCode"] = "900";
            //dic["errMsg"] = "签名校验失败";
            //          //var text = "channelId=3&method=loanApply&params=\"{\"loanId\":\"20170915172722000007\"}\"&signType = RSA2 & ver = 1.0";
            var d = dic.OrderBy(p => p.Key).ToDictionary(p => p.Key, o => o.Value);
            var text = WebUtils.BuildQuery(d, false, charset);

            //要组装成一个对象？
            JObject jb = new JObject();
            foreach (var key in dic.Keys)
            {
                jb.Add(new JProperty(key, dic[key]));
            }

            var tt = JsonConvert.SerializeObject(jb);

            //          var sss = "YtaXroGTrMptqZPQW8/Cz1ZrMGqL8s4V8JBYQr2LHh0j0WC+BP5NSZxVljBxsrBvv9vyH6l8ODX/1mT8AmYbxptbYW7RGJ0Of87CMJFrmTHb9f9nRFs3j7dNUw6PfSRuT1ItqHXAbEoUO8ZSm278yILIkpdTRJO1EjvdFH25ILs=";
            //          //"YtaXroGTrMptqZPQW8/Cz1ZrMGqL8s4V8JBYQr2LHh0j0WC+BP5NSZxVljBxsrBvv9vyH6l8ODX/1mT8AmYbxptbYW7RGJ0Of87CMJFrmTHb9f9nRFs3j7dNUw6PfSRuT1ItqHXAbEoUO8ZSm278yILIkpdTRJO1EjvdFH25ILs="
            //             //oOzbzpanqMkEqib40YcnDaw7eb296ORiEE37Ysz/XpJVeJtvqAsZ5yIseXFMsXRjJZ1yCyknuspZ5qoglIDhHkgPn/S2UBnR1f/JuyCHifxW7tJgu1CpbFdHZ7BFHwGmxb97Jx0pOYKaVKW14bTZgnLKepBStT4SjhFeX7LUPoE=
            ////var s = RSAUtil.Encrypt(text, privateKey, charset);

            //var text = "channelId=3&method=loanApply&params={\"loanId\":\"20170918151253000014\"}&signType=RSA2&ver=1.0";
            var sss = "YtaXroGTrMptqZPQW8/Cz1ZrMGqL8s4V8JBYQr2LHh0j0WC+BP5NSZxVljBxsrBvv9vyH6l8ODX/1mT8AmYbxptbYW7RGJ0Of87CMJFrmTHb9f9nRFs3j7dNUw6PfSRuT1ItqHXAbEoUO8ZSm278yILIkpdTRJO1EjvdFH25ILs=";
            //"oOzbzpanqMkEqib40YcnDaw7eb296ORiEE37Ysz/XpJVeJtvqAsZ5yIseXFMsXRjJZ1yCyknuspZ5qoglIDhHkgPn/S2UBnR1f/JuyCHifxW7tJgu1CpbFdHZ7BFHwGmxb97Jx0pOYKaVKW14bTZgnLKepBStT4SjhFeX7LUPoE="
            var t = RSAUtil.Verify(text, sss, publicKey, charset);
            Console.WriteLine($"对方验签：{t}");


            var res = "{\"statusCode\":200,\"errMsg\":\"success\",\"method\":\"loanApply\",\"ver\":\"1.0\",\"channelId\":\"3\",\"signType\":\"RSA2\",\"sign\":\"LzQ7rhhoCcn3in6J0kArQak+J0zBzFOEtiDaLNAMDpctdQYK9aEUNUlhK5hZJ9ExP+miq1AXr9EMNya1aZxibPh/MdKYnr9vaNHQk7hpUbycE9bxP1cdrFB9VgLOLQyAf8HF0njTGSK/ozCxzqrhqtT1Y+9WcQbRs7h+yLyfoFA=\",\"params\":{\"loanId\":\"20170918144652000012\"}}";

            var des_dic = JsonConvert.DeserializeObject<IDictionary<string, object>>(res);

            Console.WriteLine(des_dic);

        }

        static void signtest()
        {
            string privateKey = Cfg.Get("privateKey");
            string publicKey = Cfg.Get("publicKey");
            string myPublicKey = Cfg.Get("myPublicKey");
            string charset = "UTF-8";
            var text = "123";
            var sss = "qGv4v16jabQaVrfVwLGUup31xtuSoufwP77d0nsLV5jfGs7N12143gT0yf8ek1SQv1dtaZlliSxgyaga/Z3tUWAdaUGA8BBsBHYc1OLTzKFGHyMY1QLiokUe5xJ/lbZFuyr3L6uhGTCGqWbni/yinNEA7KEjhJVGuRXxv06s=";
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
            var tt = new { id = "123" };
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


        static void generatorsign()
        {
            //用我的私钥和公钥用来测试接口编写

            string privateKey = Cfg.Get("privateKey");
            string publicKey = Cfg.Get("publicKey");
            string myPublicKey = Cfg.Get("myPublicKey");
            string charset = "UTF-8";

            var dic = new Dictionary<string, object>();
            dic["channelId"] = "3";
            dic["method"] = "loanApplyResultNotify";
            //dic["params"] = new { loanId = "20170915174747000008" };
            var loanapplyresult = new LoanApplyResult()
            {
                LoanId = "2019125514515",
                Result = 1,
                Commissions = 10000,
                Reason = "没有拒绝",
                LoanAmount = 1000000,
                LoanTerm = 12,
                PaymentOption = 1,
                Orders = new List<ResultOrder>() {
                    new ResultOrder() { SourceOrderId="12345",LoanAmount=5000 },
                    new ResultOrder() { SourceOrderId="12346",LoanAmount=5000 }
                }
            };

            var loantttttstring = JsonConvert.SerializeObject(loanapplyresult);

            dic["params"] = loantttttstring;
            dic["signType"] = "RSA2";
            dic["ver"] = "1.0";
            //dic["statusCode"] = "900";
            //dic["errMsg"] = "签名校验失败";
            var d = dic.OrderBy(p => p.Key).ToDictionary(p => p.Key, o => o.Value);
            var text = WebUtils.BuildQuery(d, false, charset);

            //要组装成一个对象？
            JObject jb = new JObject();
            foreach (var key in dic.Keys)
            {
                jb.Add(new JProperty(key, dic[key]));
            }

            var tt = JsonConvert.SerializeObject(jb);
            var s = RSAUtil.Sign(text, privateKey, charset);

            Console.WriteLine($"签名：{s}");

        }
    }
}
