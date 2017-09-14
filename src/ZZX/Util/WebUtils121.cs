using DotNet.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Wx.Utilities.HttpUtility
{
    public static class WebUtils
    {

        private static readonly string DefaultUserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/31.0.1650.57 Safari/537.36";

        private static readonly string DefaultContentType = "application/json;charset=UTF-8";

        private static readonly string DefaultAccept = "application/json, text/plain, */*";

        private static readonly string DefaultEncoding = "gzip, deflate, br";

        private static readonly string DefaultLanguage = "zh-CN,zh;q=0.8";


        #region  辅助方法

        private static HttpItem NewHttpItem()
        {
            HttpItem item = new HttpItem();
            item.Accept = DefaultAccept;
            item.ContentType = DefaultContentType;
            item.UserAgent = DefaultUserAgent;

            return item;
        }

        ///// <summary>
        ///// 组装QueryString的方法
        ///// 参数之间用&amp;连接，首位没有符号，如：a=1&amp;b=2&amp;c=3
        ///// </summary>
        ///// <param name="formData"></param>
        ///// <returns></returns>
        //public static string GetQueryString(this Dictionary<string, string> formData)
        //{
        //    if (formData == null || formData.Count == 0)
        //    {
        //        return "";
        //    }

        //    StringBuilder sb = new StringBuilder();

        //    var i = 0;
        //    foreach (var kv in formData)
        //    {
        //        i++;
        //        sb.AppendFormat("{0}={1}", kv.Key, kv.Value);
        //        if (i < formData.Count)
        //        {
        //            sb.Append("&");
        //        }
        //    }

        //    return sb.ToString();
        //}

        ///// <summary>
        ///// 封装System.Web.HttpUtility.UrlEncode
        ///// </summary>
        ///// <param name="url"></param>
        ///// <returns></returns>
        //public static string UrlEncode(this string url)
        //{
        //    return System.Web.HttpUtility.UrlEncode(url);
        //}
        ///// <summary>
        ///// 封装System.Web.HttpUtility.UrlDecode
        ///// </summary>
        ///// <param name="url"></param>
        ///// <returns></returns>
        //public static string UrlDecode(this string url)
        //{
        //    return System.Web.HttpUtility.UrlDecode(url);
        //}

        /// <summary>
        /// 组装普通文本请求参数。
        /// </summary>
        /// <param name="parameters">Key-Value形式请求参数字典</param>
        /// <returns>URL编码后的请求数据</returns>
        public static string GetQueryString(this IDictionary<string, string> parameters, string charset)
        {
            return parameters.GetQueryString(true, charset);
        }

        public static string GetQueryString(this IDictionary<string, string> parameters, bool encode, string charset)
        {
            StringBuilder postData = new StringBuilder();
            bool hasParam = false;

            IEnumerator<KeyValuePair<string, string>> dem = parameters.GetEnumerator();
            while (dem.MoveNext())
            {
                string name = dem.Current.Key;
                string value = dem.Current.Value;
                // 忽略参数名或参数值为空的参数
                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(value))
                {
                    if (hasParam)
                    {
                        postData.Append("&");
                    }

                    postData.Append(name);
                    postData.Append("=");

                    if (encode)
                    {
                        value = System.Web.HttpUtility.UrlEncode(value, Encoding.GetEncoding(charset));
                    }

                    postData.Append(value);
                    hasParam = true;
                }
            }

            return postData.ToString();

        }

        public static string GetQueryStringWithoutEncode(this IDictionary<string, string> parameters)
        {
            return parameters.GetQueryString(false, null);
        }

        #endregion

        public static string HttpGetString(string url)
        {
            HttpItem item = NewHttpItem();
            item.URL = url;
            HttpHelper http = new HttpHelper();
            HttpResult result = http.GetHtml(item);
            return result.Html;
        }

        public static byte[] HttpGetByte(string url)
        {
            HttpItem item = NewHttpItem();
            item.URL = url;
            item.ResultType = ResultType.Byte;

            HttpHelper http = new HttpHelper();
            HttpResult result = http.GetHtml(item);
            return result.ResultByte;
        }

        public static string HttpPost(string url, string body = "")
        {
            HttpItem item = NewHttpItem();
            item.URL = url;
            item.Method = "Post";
            item.PostDataType = PostDataType.String;
            item.Postdata = body;
            item.Encoding = Encoding.UTF8;
            item.PostEncoding = Encoding.UTF8;//发送消息编码出问题了 这里测试一下

            HttpHelper http = new HttpHelper();
            HttpResult result = http.GetHtml(item);

            return result.Html;
        }


        public static string HttpPost(string url, Dictionary<string, string> formData = null)
        {
            HttpItem item = NewHttpItem();
            item.URL = url;
            item.Method = "Post";
            item.PostDataType = PostDataType.String;
            item.Postdata = formData.GetQueryString("UTF-8");
            item.Encoding = Encoding.UTF8;
            item.PostEncoding = Encoding.UTF8;

            HttpHelper http = new HttpHelper();
            HttpResult result = http.GetHtml(item);

            return result.Html;
        }





    }
}
