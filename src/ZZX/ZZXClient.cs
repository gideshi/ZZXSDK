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
        //private string _serverUrl;
        //private string _method;
        //private const string _ver = "v1";
        //private string _channelId;
        //private string _defaultCharset;
        //private const string _signType = "RSA2";
        //private string _sign;
        //private string _params;

        //private string _unSignedString;
        //private string _signedString;

        public const string METHOD = "method";
        public const string VERSION = "ver";
        public const string ChANNELID = "channelId";
        public const string SIGNTYPE = "signType";
        public const string SIGN = "sign";
        public const string PARAMS = "params";
        public const string CHARSET = "charset";

        //private Dictionary<string, string> _formDic;

        private string _version;
        private string _serverUrl;
        //私钥
        private string _privateKey;

        //公钥
        private string _publicKey;

        private string _channelId;

        //默认编码
        private string _charset = ZZXConstants.DEFAULT_CHARSET;
        //算法名称
        private string _singType = ZZXConstants.SIGNTYPE;

        private WebUtils _webUtils;

        public string Version
        {
            get { return _version != null ? _version : "1.0"; }
            set { _version = value; }
        }

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
            ZZXDictionary sysParams = getSystemParams(request);
            string body;

            //这里要组装成对象
            JObject jb = new JObject();
            foreach (var key in sysParams.Keys)
            {
                //params 这个要转回问题
                if (key == "params")
                    jb.Add(new JProperty(key, JsonConvert.DeserializeObject(sysParams[key].ToString())));
                else
                    jb.Add(new JProperty(key, sysParams[key]));
            }
            var tt = JsonConvert.SerializeObject(jb);
            var encode = HttpUtility.UrlEncode(tt);//传递的时候进行url编码
            body = _webUtils.DoPost(_serverUrl, encode, _charset);
            string bizResponse = body;
            T rsp = null;

            //再这里转换出来然后验签
            ZZXDictionary dic = new ZZXDictionary();
            JObject jObject = JsonConvert.DeserializeObject(bizResponse) as JObject;
            if (jObject != null)
            {
                //去掉 statuscode errmsg  sign 三个键值对 排序组合成待签名字符串
                if (jObject["sign"] != null)
                {
                    var sign = jObject["sign"].ToString();
                    dic.Add("method", jObject["method"].ToString());
                    dic.Add("ver", jObject["ver"].ToString());
                    dic.Add("channelId", jObject["channelId"].ToString());
                    dic.Add("signType", jObject["signType"].ToString());
                    if (jObject["params"] != null)
                        dic.Add("params", JsonConvert.SerializeObject(jObject["params"]));
                    var d = dic.OrderBy(p => p.Key).ToDictionary(p => p.Key, o => o.Value);
                    var s = WebUtils.BuildQuery(d, false, _charset);
                    RSAUtil.VerifySign(s, sign, _publicKey, _charset);
                }
            }

            rsp = JsonConvert.DeserializeObject<T>(bizResponse);
            return rsp;
        }

        public ZZXDictionary getSystemParams<T>(IZZXRequest<T> request) where T : ZZXResponse
        {
            string apiVersion = null;
            if (!string.IsNullOrEmpty(request.GetApiVersion()))
            {
                apiVersion = request.GetApiVersion();
            }
            else
            {
                apiVersion = Version;
            }
            ZZXDictionary sysParams = new ZZXDictionary();
            sysParams.Add(METHOD, request.GetApiName());
            sysParams.Add(VERSION, apiVersion);
            sysParams.Add(ChANNELID, _channelId);
            sysParams.Add(SIGNTYPE, _singType);
            sysParams.Add(PARAMS, request.GetParams());
            var d = sysParams.OrderBy(p => p.Key).ToDictionary(p => p.Key, o => o.Value);//签名需要先排序下 中子星文档要求
            // 添加签名参数
            var build = WebUtils.BuildQuery(d, false, _charset);//  这个签名没问题
            sysParams.Add(SIGN, RSAUtil.Sign(WebUtils.BuildQuery(d, false, _charset), _privateKey, _charset));
            return sysParams;
        }




    }
}
