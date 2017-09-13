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
            string privateKey = Cfg.Get("privateKey");
            string publicKey = Cfg.Get("publicKey");
            string charset = "UTF-8";
            var text = "123";
            var sss = "mLk38/34Cv/O2514rMkK4fTsmaSD04YhWqM7cWp6Cxq9Vi6t4jySYIOglmRvW09bq9+UcmNCMPgVCrq0Zu06a9Mw7EXq3eHcULQTjL3QNJGzkjq27nnl2axsZRpg47vJSc9h5YGgCsFQeO+gVYDNN/0QX79PpdudyytB7kHVFtk=";
            //var s = RSAUtil.Encrypt(text, privateKey, charset);

            var s = RSAUtil.Sign(text,privateKey,charset);

            Console.Write(s);

            //var t=RSAUtil.Verify("123",sss,publicKey,charset);
            //Console.WriteLine(t);
            //SignTest();
            //bctest();
            Console.ReadLine();
        }

        

        static void bctest()
        {

            //RSA密钥对的构造器 
            RsaKeyPairGenerator keyGenerator = new RsaKeyPairGenerator();

            //RSA密钥构造器的参数 
            RsaKeyGenerationParameters param = new RsaKeyGenerationParameters(
                Org.BouncyCastle.Math.BigInteger.ValueOf(3),
                new Org.BouncyCastle.Security.SecureRandom(),
                1024,   //密钥长度 
                25);
            //用参数初始化密钥构造器 
            keyGenerator.Init(param);
            //产生密钥对 
            AsymmetricCipherKeyPair keyPair = keyGenerator.GenerateKeyPair();
            //获取公钥和密钥 
            AsymmetricKeyParameter publicKey = keyPair.Public;
            AsymmetricKeyParameter privateKey = keyPair.Private;
            if (((RsaKeyParameters)publicKey).Modulus.BitLength < 1024)
            {
                Console.WriteLine("failed key generation (1024) length test");
            }
            //一个测试…………………… 
            //输入，十六进制的字符串，解码为byte[] 
            //string input = "4e6f77206973207468652074696d6520666f7220616c6c20676f6f64206d656e"; 
            //byte[] testData = Org.BouncyCastle.Utilities.Encoders.Hex.Decode(input);            
            string input = "123";
            byte[] testData = Encoding.UTF8.GetBytes(input);
            Console.WriteLine("明文:" + input + Environment.NewLine);
            //非对称加密算法，加解密用 
            IAsymmetricBlockCipher engine = new RsaEngine();
            //公钥加密 
            engine.Init(true, publicKey);
            try
            {
                testData = engine.ProcessBlock(testData, 0, testData.Length);
                Console.WriteLine("密文（base64编码）:" + Convert.ToBase64String(testData) + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine("failed - exception " + Environment.NewLine + ex.ToString());
            }
            //私钥解密 
            engine.Init(false, privateKey);
            try
            {
                testData = engine.ProcessBlock(testData, 0, testData.Length);

            }
            catch (Exception e)
            {
                Console.WriteLine("failed - exception " + e.ToString());
            }
            if (input.Equals(Encoding.UTF8.GetString(testData)))
            {
                Console.WriteLine("解密成功");
            }
            Console.Read();
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

            using (HttpClient client = new HttpClient())
            {
                IEnumerable<KeyValuePair<string, string>> queryPart = new List<KeyValuePair<string, string>>(){
                                    new KeyValuePair<string, string>("params",parms),
                                    new KeyValuePair<string, string>("sign",sign)
                                };
                HttpContent q = new FormUrlEncodedContent(queryPart);
                //url = baseurl + url;

                using (HttpResponseMessage response = client.PostAsync("https://ssl-scf.xingyoucai.com/api/v1/antai/checkSign.do", q).Result)
                {
                    using (HttpContent content = response.Content)
                    {
                        var html = content.ReadAsStringAsync().Result;
                        Console.WriteLine(html);
                    }
                }

            }

            //HttpClient client = new HttpClient();
            //var t = client.PostAsync("https://ssl-scf.xingyoucai.com/api/v1/antai/checkSign.do?params=" + parms + "&sign=" + sign).Result;



            //var body = JsonConvert.SerializeObject(dic);

            //var result = RequestHelper.HttpPost("https://ssl-scf.xingyoucai.com/api/v1/antai/checkSign.do", body: body);

            //Console.WriteLine(t);

        }
    }
}
