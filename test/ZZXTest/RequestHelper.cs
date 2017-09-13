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
    public static class RequestHelper
    {

        public static CookieCollection CookieCollection = new CookieCollection();

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
            item.CookieCollection = CookieCollection;

            return item;
        }

        /// <summary>
        /// 获取所有cookie
        /// </summary>
        /// <returns></returns>
        private static List<Cookie> GetAllCookies()
        {
            List<Cookie> lstCookies = new List<Cookie>();
            //Hashtable table = (Hashtable)cc.GetType().InvokeMember("m_domainTable",
            //     System.Reflection.BindingFlags.NonPublic |
            //      System.Reflection.BindingFlags.GetField |
            //       System.Reflection.BindingFlags.Instance,
            //       null, cc, new object[] { });

            //foreach (object pathList in table.Values)
            //{
            //    SortedList lstCookieCol = (SortedList)pathList.GetType().InvokeMember("m_list",
            //         System.Reflection.BindingFlags.NonPublic |
            //          System.Reflection.BindingFlags.GetField |
            //           System.Reflection.BindingFlags.Instance,
            //           null, pathList, new object[] { });

            //    foreach (CookieCollection colCoolies in lstCookieCol.Values)
            //        foreach (Cookie c in colCoolies)
            //            lstCookies.Add(c);
            //}

            foreach (Cookie c in CookieCollection)
                lstCookies.Add(c);

            return lstCookies;
        }

        /// <summary>
        /// 获取指定 cookie
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Cookie GetCookie(string name)
        {
            return GetAllCookies().First(o => o.Name == name);
        }

        public static void SetCookie(Cookie cookie)
        {
            CookieCollection.Add(cookie);
        }

        public static void SetCookie(CookieCollection cookieCollection)
        {
            foreach (Cookie cookie in cookieCollection)
            {
                SetCookie(cookie);
            }
        }

        public static void SetCookie(string cookieString)
        {
            if (cookieString.GetString().Length > 0)
            {
                var regex = new Regex(@"\S+[=]\S+;.*?[Domain=]\S+;.*?[Path=]\S+;.*?[Expires=]\S+,.*?GMT", RegexOptions.IgnoreCase);
                var arr_cookie =regex.Matches(cookieString);
                
                Cookie cookie = null;
                regex = new Regex(@",{0,1}.*?(?<name>\w+)=(?<value>\S+);.*?Domain=(?<Domain>\S+);.*?Path=(?<Path>\S+);.*?Expires=(?<Expires>.*?GMT)", RegexOptions.IgnoreCase);
                foreach (Match s in arr_cookie)
                {
                    var match = regex.Match(s.Value);

                    cookie = new Cookie();
                    cookie.Name = match.Groups["name"].Value;
                    cookie.Value = match.Groups["value"].Value;
                    cookie.Domain = match.Groups["Domain"].Value;
                    cookie.Path = match.Groups["Path"].Value;
                    cookie.Expires = DateTime.Parse(match.Groups["Expires"].Value);
                    CookieCollection.Add(cookie);
                }
                
            }
        }

        /// <summary>
        /// 组装QueryString的方法
        /// 参数之间用&amp;连接，首位没有符号，如：a=1&amp;b=2&amp;c=3
        /// </summary>
        /// <param name="formData"></param>
        /// <returns></returns>
        public static string GetQueryString(this Dictionary<string, string> formData)
        {
            if (formData == null || formData.Count == 0)
            {
                return "";
            }

            StringBuilder sb = new StringBuilder();

            var i = 0;
            foreach (var kv in formData)
            {
                i++;
                sb.AppendFormat("{0}={1}", kv.Key, kv.Value);
                if (i < formData.Count)
                {
                    sb.Append("&");
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// 封装System.Web.HttpUtility.UrlEncode
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string UrlEncode(this string url)
        {
            return System.Web.HttpUtility.UrlEncode(url);
        }
        /// <summary>
        /// 封装System.Web.HttpUtility.UrlDecode
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string UrlDecode(this string url)
        {
            return System.Web.HttpUtility.UrlDecode(url);
        }

        #endregion

        public static string HttpGetString(string url)
        {
            HttpItem item = NewHttpItem();
            item.URL = url;
            //item.Method = "Get";//默认Get

            HttpHelper http = new HttpHelper();

            HttpResult result = http.GetHtml(item);
            SetCookie(result.Cookie);//每次请求都重新赋值一次做到每次cookie 都是最新的
            return result.Html;
        }

        public static byte[] HttpGetByte(string url)
        {
            HttpItem item = NewHttpItem();
            item.URL = url;
            //item.Method = "Get";
            item.ResultType = ResultType.Byte;

            HttpHelper http = new HttpHelper();
            HttpResult result = http.GetHtml(item);
            SetCookie(result.Cookie);
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
            SetCookie(result.Cookie);
            return result.Html;
        }


        public static string HttpPost(string url, Dictionary<string, string> formData = null)
        {
            HttpItem item = NewHttpItem();
            item.URL = url;
            item.Method = "Post";
            item.PostDataType = PostDataType.String;
            item.Postdata = formData.GetQueryString();
            item.Encoding = Encoding.UTF8;
            item.PostEncoding = Encoding.UTF8;

            HttpHelper http = new HttpHelper();
            HttpResult result = http.GetHtml(item);
            SetCookie(result.Cookie);
            return result.Html;
        }





    }
}
