using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZZX;
using ZZX.Request;
using ZZX.Response;
using ZZX.Util;

namespace ZZXTest
{
    class Program
    {
        static void Main(string[] args)
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

            ZZXClient client = new ZZXClient(url, channelId, "loanApply", privateKey, publicKey, charset);

            LoanApplyRequest request = new LoanApplyRequest();

            //request //设置请求参数
            LoanApplyResponse response = client.Execute(request);

            Console.WriteLine(response);

            Console.ReadLine();
        }
    }
}
