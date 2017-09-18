using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZZX;
using ZZX.Entities;
using ZZX.Request;
using ZZX.Response;

namespace ZZXSDK.Tests
{
    [TestClass]
    public class ZZXTest
    {
        string ss = "";
        string url = "https://ssl-scf.xingyoucai.com/api/v1/antai/entry.do";
        string channelId = "3";
        string privateKey = "MIICdgIBADANBgkqhkiG9w0BAQEFAASCAmAwggJcAgEAAoGBAPG+IIarMp0uURT+/Q5NcvoJUxP6j8h5uYncly79WFjJhxOheuazYEU+Q/S/I+FWWR4UP6A3qmpclZksvq/WBZ33MHStxj6ALpgWqGaMnoNRxk8EBeWFuj61H4J45KxJ35JHlhby6G6d3GAUezlDMETSEe92zCr829y9NbUyujENAgMBAAECgYAXXDUObmq0r64cJkvT3v4WVWJW0uakC8c3ID7nxomMAuVvqzISKxFJf6vXccUI2GxCMNi5JcftAUdfhuhiW38tRYbV+DeSNl2qKPUlIXlSifJcZTQbWm9ecBceEFyANeZ99RbZE6EVirKMP1pLvgx2UzLmxCpqeescE3rYoeI9gQJBAP7uXJA9XBVv+EgtCkXXd/LCCkJdDPX3xN7ys8Vjv7/8StPVhpLRun2H2gcOR7O33ek6bvp7FO4WnTBx1/UwCu0CQQDywZv5QNU91uW2qQj6zdXCVymmbzhsZo8KTGmAqzlTkjIqg2Wg9h2ZZNtZcImT/UxdfnN3dWaRMUJbDDeY3FqhAkAjwHv8wo4yd3SDcsWZC+HHiszzh6c0q53RgooRqa6PlytLUAvCdWVJC49ZI/iMTMHzXn2H5VEHHubGj4Cw4x71AkEAqXWX4P5uHVYHU2RaXWIUxy47Z/CZ7aoGTkUbHPPp97nFhUHmTt+ft/Xc3Wme0Izwow9joU92AwLk9FPp965TIQJAZQ1BrCruivSrLyiqB9nAVx8iSkD6hFCfApWeWF4l95SeWDTQMhem75AKyjaQD2ns0aEGUfrWY7L6R68aTV9Vow==";
        string publicKey = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCzamRTrfZqZIn7AUGJnu3USYpgFw9vODHxRHgDI8wVh3Tg49FvsweSWONrsFyxRR5Jv1UQ3S78NiRg3sObcI3N8ERlEgeTdAYmVN/061XRXH5GO5eNd6gN1uYRqflZaL6B1hJ3nPhnnqDmoiCmbl6PXe0QHrOEGrqKqF4lEG52DwIDAQAB";
        string charset = "UTF-8";

        #region     LoabApply  贷款申请接口
       
        [TestMethod]
        public void LoanApplyFail()
        {
            ZZXClient zzxclient = new ZZXClient(url, channelId, privateKey, publicKey, charset);
            ZZXApiRequest request = new ZZXApiRequest();
            request.Method = "loanApply";

            var orders = new List<Order>();
            Order order = null;
            for (int i = 1; i <10; i++)
            {
                order = new Order();
                order.OrderId =$"DDBH{i}";
                order.Name = $"订单名称{i}";
                order.OrganizationId = $"DWBH{i}";
                order.Organization = $"单位名称{i}";
                order.Mobile = $"1590000000{i}";
                order.CardNO = $"32038100000000000{i}";
                order.DivideRate =i/10;
                order.Level = $"套餐档次{i}";
                order.OrderDate = $"2017-09-1{i}";
                order.PackageDuration = "24";
                order.Type = "iPhone X";

                orders.Add(order);
            }

            var parms = new {
                amount=100000,
                //productId= "ef0fa7b2e2564f3fb8308caac4be90c0",
                productId="123",
                orders =orders
            };

            request.Params = JsonConvert.SerializeObject(parms);
            ZZXApiResponse response = zzxclient.Execute(request);

            Assert.AreNotEqual(response.StatusCode, 200);
        }

        [TestMethod]
        public void LoanApplySuccess()
        {
            ZZXClient zzxclient = new ZZXClient(url, channelId, privateKey, publicKey, charset);
            ZZXApiRequest request = new ZZXApiRequest();
            request.Method = "loanApply";

            var orders = new List<Order>();
            Order order = null;
            for (int i = 1; i < 10; i++)
            {
                order = new Order();
                order.OrderId = $"DDBH{i}";
                order.Name = $"订单名称{i}";
                order.OrganizationId = $"DWBH{i}";
                order.Organization = $"单位名称{i}";
                order.Mobile = $"1590000000{i}";
                order.CardNO = $"32038100000000000{i}";
                order.DivideRate = i / 10;
                order.Level = $"套餐档次{i}";
                order.OrderDate = $"2017-09-1{i}";
                order.PackageDuration = "24";
                order.Type = "iPhone X";

                orders.Add(order);
            }

            var parms = new
            {
                amount = 100000,
                productId = "ef0fa7b2e2564f3fb8308caac4be90c0",
                orders = orders
            };

            request.Params = JsonConvert.SerializeObject(parms);
            ZZXApiResponse response = zzxclient.Execute(request);

            dynamic t= response.Params;

            //var tt = t.loanId;

            Assert.AreEqual(response.StatusCode, 200);
        }

        #endregion

        #region  
        #endregion
    }
}
