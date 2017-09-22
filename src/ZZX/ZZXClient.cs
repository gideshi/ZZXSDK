using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ZZX.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ZZX
{
    public class ZZXClient : IZZXClient
    {
        private string _serverUrl;
        //私钥
        private string _privateKey;
        //公钥
        private string _publicKey;
        //平台编号
        private string _channelId;
        //版本
        private string _version = ZZXConstants.DEFAULT_VERSION;
        //默认编码
        private string _charset = ZZXConstants.DEFAULT_CHARSET;
        //签名算法
        private string _singType = ZZXConstants.SIGNTYPE;

        private WebUtils _webUtils;

        public ZZXClient(string serverUrl, string channelId, string privateKey, string publicKey)
        {
            _serverUrl = serverUrl;
            _privateKey = privateKey;
            _publicKey = publicKey;
            _channelId = channelId;
            _webUtils = new WebUtils();
        }

        public ZZXClient(string serverUrl, string channelId, string privateKey, string publicKey, string charset)
        {
            _serverUrl = serverUrl;
            _privateKey = privateKey;
            _publicKey = publicKey;
            _channelId = channelId;
            _charset = charset;

            _webUtils = new WebUtils();
        }

        public T Execute<T>(IZZXRequest<T> request) where T : ZZXResponse
        {
            if (string.IsNullOrEmpty(_charset))
            {
                _charset = "UTF-8";
            }
            CompRequest(ref request);//组装参数
            AddSign(ref request);//添加签名
            var jSetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            var json_request = JsonConvert.SerializeObject(request, Formatting.None, jSetting);
            var encode_request = HttpUtility.UrlEncode(json_request);//传递的时候进行url编码
            var body = _webUtils.DoPost(_serverUrl, encode_request, _charset);
            T rsp = null;
            JObject jObject = JsonConvert.DeserializeObject(body) as JObject;
            VerifySign(jObject);

            rsp = JsonConvert.DeserializeObject<T>(body);
            return rsp;
        }

        /// <summary>
        /// request 参数组装
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        private void CompRequest<T>(ref IZZXRequest<T> request) where T : ZZXResponse
        {
            if (string.IsNullOrEmpty(request.ChannelId))
            {
                request.ChannelId = _channelId;
            }
            if (string.IsNullOrEmpty(request.SignType))
            {
                request.SignType = _singType;
            }
            if (string.IsNullOrEmpty(request.Ver))
            {
                request.Ver = _version;
            }
        }

        public void AddSign<T>(ref IZZXRequest<T> request) where T : ZZXResponse
        {
            ZZXDictionary dic = request.GetParameters() as ZZXDictionary;
            //剔除sign
            dic.Remove("sign");
            var d = dic.OrderBy(p => p.Key).ToDictionary(p => p.Key, o => o.Value);//签名需要先排序下 中子星文档要求
            JObject jobject = new JObject();
            foreach (var m in d)
            {
                jobject.Add(m.Key, JToken.FromObject(m.Value));
            }
            //先序列化一次


            // 添加签名参数
            var un_sign = WebUtils.BuildQueryT(jobject, false, _charset);
            var sign = RSAUtil.Sign(un_sign, _privateKey, _charset);
            request.Sign = sign;
        }

        public void VerifySign(JObject jobject)
        {
            if (jobject != null)
            {
                if (jobject["sign"] != null)
                {
                    ZZXDictionary dic = new ZZXDictionary();
                    IEnumerable<JProperty> properties = jobject.Properties();
                    foreach (JProperty item in properties)
                    {
                        //null 不参与签名
                        if (item.HasValues && item.Value.Type != JTokenType.Null)
                        {
                            //全部入集合
                            dic.Add(item.Name, item.Value);
                        }
                    }

                    //剔除不参与签名的三个参数
                    dic.Remove("statusCode");
                    dic.Remove("errMsg");
                    dic.Remove("sign");
                    var d = dic.OrderBy(p => p.Key).ToDictionary(p => p.Key, o => o.Value);//签名需要先排序下 中子星文档要求
                    JObject jo = new JObject();
                    foreach (var m in d)
                    {
                        jo.Add(m.Key, JToken.FromObject(m.Value));
                    }

                    var sign = jobject["sign"].ToString();
                    var s = WebUtils.BuildQueryT(jo, false, _charset);
                    RSAUtil.VerifySign(s, sign, _publicKey, _charset);
                }
                else
                {
                    throw new ZZXException("check sign failed: " + JsonConvert.SerializeObject(jobject));
                }

            }
            else
            {
                throw new ZZXException("check sign failed: response is null");
            }
        }



    }
}
