using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        string loadId = "";

        #region     3rd 主动

        #region     LoabApply  贷款申请接口

        [TestMethod]
        public void LoanApplyFail()
        {
            ZZXClient zzxclient = new ZZXClient(url, channelId, privateKey, publicKey, charset);
            ZZXApiRequest request = new ZZXApiRequest();
            request.Method = "loanApply";

            var orders = new List<Order>();
            Order order = null;
            for (int i = 1; i < 3; i++)
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
                //productId= "ef0fa7b2e2564f3fb8308caac4be90c0",
                productId = "123",
                orders = orders
            };

            request.Params = JObject.FromObject(parms) ;
            ZZXApiResponse response = zzxclient.Execute(request);

            Assert.AreNotEqual(response.StatusCode, 200);
        }

        [TestMethod]
        public void LoanApplySuccess()
        {
            //ZZXClient zzxclient = new ZZXClient(url, channelId, privateKey, publicKey, charset);
            //ZZXApiRequest request = new ZZXApiRequest();
            //request.Method = "loanApply";

            //var orders = new List<Order>();
            //Order order = null;
            //for (int i = 1; i < 10; i++)
            //{
            //    order = new Order();
            //    order.OrderId = $"DDBH{i}";
            //    order.Name = $"订单名称{i}";
            //    order.OrganizationId = $"DWBH{i}";
            //    order.Organization = $"单位名称{i}";
            //    order.Mobile = $"1590000000{i}";
            //    order.CardNO = $"32038100000000000{i}";
            //    order.DivideRate = i / 10;
            //    order.Level = $"套餐档次{i}";
            //    order.OrderDate = $"2017-09-1{i}";
            //    order.PackageDuration = "24";
            //    order.Type = "iPhone X";

            //    orders.Add(order);
            //}

            //var parms = new
            //{
            //    amount = 100000,
            //    productId = "ef0fa7b2e2564f3fb8308caac4be90c0",
            //    orders = orders
            //};

            //request.Params = JsonConvert.SerializeObject(parms);
            //ZZXApiResponse response = zzxclient.Execute(request);

            //dynamic t = response.Params;

            ////var tt = t.loanId;
            //loadId = t.loanId;

            //Assert.AreEqual(response.StatusCode, 200);
        }

        #endregion

        #region  uploadAttachment 上传申请单位资料 

        [TestMethod]
        public void UploadAttachmentFail()
        {
            var tmp_loadId = "123";
            ZZXClient zzxclient = new ZZXClient(url, channelId, privateKey, publicKey, charset);
            ZZXApiRequest request = new ZZXApiRequest();
            request.Method = "uploadAttachment";

            var parms = new
            {
                id = tmp_loadId,
                filename = "test.jpg",
                data = "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"
            };
            request.Params = JsonConvert.SerializeObject(parms);
            ZZXApiResponse response = zzxclient.Execute(request);

            Assert.AreNotEqual(response.StatusCode, 200);
        }



        [TestMethod]
        public void UploadAttachmentSuccess()
        {
            //20170919091725000037
            var tmp_loadId = "20170919091725000037";
            ZZXClient zzxclient = new ZZXClient(url, channelId, privateKey, publicKey, charset);
            ZZXApiRequest request = new ZZXApiRequest();
            request.Method = "uploadAttachment";

            var parms = new
            {
                id = tmp_loadId,
                filename = "test.jpg",
                data = "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"
            };
            request.Params = JsonConvert.SerializeObject(parms);
            ZZXApiResponse response = zzxclient.Execute(request);

            Assert.AreEqual(response.StatusCode, 200);
        }

        #endregion

        #region  确认贷款方案

        [TestMethod]
        public void loanApplySubmitFail()
        {
            var tmp_loadId = "123";
            ZZXClient zzxclient = new ZZXClient(url, channelId, privateKey, publicKey, charset);
            ZZXApiRequest request = new ZZXApiRequest();
            request.Method = "loanApplySubmit";

            var parms = new
            {
                loanId = tmp_loadId
            };

            request.Params = JsonConvert.SerializeObject(parms);
            ZZXApiResponse response = zzxclient.Execute(request);

            Assert.AreNotEqual(response.StatusCode, 200);
        }

        [TestMethod]
        public void loanApplySubmitSuccess()
        {
            var tmp_loadId = "20170919095249000038";
            ZZXClient zzxclient = new ZZXClient(url, channelId, privateKey, publicKey, charset);
            ZZXApiRequest request = new ZZXApiRequest();
            request.Method = "loanApplySubmit";

            var parms = new
            {
                loanId = tmp_loadId
            };

            request.Params = JsonConvert.SerializeObject(parms);
            ZZXApiResponse response = zzxclient.Execute(request);

            Assert.AreEqual(response.StatusCode, 200);
        }


        #endregion

        #region     loanContractConfirm 贷款申请结果确认

        [TestMethod]
        public void loanContractConfirmFail()
        {
            var tmp_loadId = "123";
            ZZXClient zzxclient = new ZZXClient(url, channelId, privateKey, publicKey, charset);
            ZZXApiRequest request = new ZZXApiRequest();
            request.Method = "loanContractConfirm";

            var parms = new
            {
                loanId = tmp_loadId,
                confirmation = 2
            };

            request.Params = JsonConvert.SerializeObject(parms);
            ZZXApiResponse response = zzxclient.Execute(request);

            Assert.AreNotEqual(response.StatusCode, 200);
        }

        [TestMethod]
        public void loanContractConfirmSuccess()
        {
            var tmp_loadId = "20170919091725000037";
            ZZXClient zzxclient = new ZZXClient(url, channelId, privateKey, publicKey, charset);
            ZZXApiRequest request = new ZZXApiRequest();
            request.Method = "loanContractConfirm";

            var parms = new
            {
                loanId = tmp_loadId,
                confirmation = 2
            };

            request.Params = JsonConvert.SerializeObject(parms);
            ZZXApiResponse response = zzxclient.Execute(request);

            Assert.AreNotEqual(response.StatusCode, 200);
        }


        #endregion

        #region  refundNotify 主动还款通知

        [TestMethod]
        public void refundNotify()
        {
            //var tmp_loadId = "20170919091725000037";
            //ZZXClient zzxclient = new ZZXClient(url, channelId, privateKey, publicKey, charset);
            //ZZXApiRequest request = new ZZXApiRequest();
            //request.Method = "refundNotify";

            //var parms = new RefundNotify()
            //{
            //    RefundType = (int)RefundType.到期正常还款,
            //    LoanId = tmp_loadId,
            //    Amount = 100000,
            //    ReriodNumber = 1
            //};

            //request.Params = JsonConvert.SerializeObject(parms);
            //ZZXApiResponse response = zzxclient.Execute(request);

            //Assert.AreNotEqual(response.StatusCode, 200);
        }


        #endregion

        #endregion

        #region   3rd被动
        

          
        #endregion

    }
}
