using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ZZX.Parser;
using ZZX.Util;
using Newtonsoft.Json;

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

            //body = WebUtils2.HttpPost(_serverUrl, JsonConvert.SerializeObject(sysParams));
            body = _webUtils.DoPost(_serverUrl, JsonConvert.SerializeObject(sysParams), _charset);

            //string bizResponse = RSAUtil.ParseBizResponse(body, _privateKey, _charset);
            string bizResponse = body;
            T rsp = null;
            IZZXParser<T> parser = null;

            //如果验签失败则抛出异常
            RSAUtil.VerifySign(body, bizResponse, _publicKey, _charset);

            parser = new ZZXJsonParser<T>();
            rsp = parser.ParseBizObj(bizResponse, _charset);
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
            sysParams.Add(SIGN, RSAUtil.Sign(WebUtils.BuildQuery(d, false, _charset), _privateKey, _charset));
            return sysParams;
        }

        public string decryptAndVerifySign(string encryptedResponse, string sign)
        {
            string decryptedResponse = RSAUtil.Decrypt(encryptedResponse, _privateKey, _charset);
            bool success = RSAUtil.Verify(decryptedResponse, sign, _publicKey, _charset);

            if (success == false)
            {
                throw new ZZXException("check sign failed: " + decryptedResponse);
            }
            return decryptedResponse;
        }


        //先得到未签名字符串
        private void BuildParams()
        {
            //_formDic = new Dictionary<string, string>();
            //_formDic["method"] = _method;
            //_formDic["ver"] = _ver;
            //_formDic["channelId"] = _channelId;
            //_formDic["signType"] = _signType;
            //_formDic["sign"] = _sign;
            //_formDic["params"] = _params;
            //_formDic = _formDic.OrderBy(p => p.Key).ToDictionary(p => p.Key, o => o.Value);//排序
        }

    }
}
