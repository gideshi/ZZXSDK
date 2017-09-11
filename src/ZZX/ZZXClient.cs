using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ZZX.Parser;
using ZZX.Util;

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

        private WebUtils _webUtils;

        public string Version
        {
            get { return _version != null ? _version : "1.0"; }
            set { _version = value; }
        }

        public ZZXClient(string serverUrl, string channelId, string method,string privateKey, string publicKey)
        {
            _serverUrl = serverUrl;
            _privateKey = privateKey;
            _publicKey = publicKey;
            _channelId = channelId;
            _webUtils = new WebUtils();
        }

        public ZZXClient(string serverUrl, string channelId, string method,string privateKey, string publicKey, string charset)
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
            return Execute<T>(request, null);
        }

        public T Execute<T>(IZZXRequest<T> request, string accessToken) where T : ZZXResponse
        {
            if (string.IsNullOrEmpty(_charset))
            {
                _charset = "UTF-8";
            }


            ZZXDictionary sysParams = getSystemParams(request);

            // 是否需要上传文件
            string body;
            if (request is IZZXUploadRequest<T>)
            {
                IZZXUploadRequest<T> uRequest = (IZZXUploadRequest<T>)request;
                IDictionary<string, FileItem> fileParams = ZZXUtils.CleanupDictionary(uRequest.GetFileParameters());
                body = _webUtils.DoPost(_serverUrl, sysParams, fileParams, _charset);
            }
            else
            {
                //body = webUtils.DoPost(serverUrl, sysParams, charset);
                body = "LA";
            }

            string bizResponse = RSAUtil.ParseBizResponse(body, _privateKey, _charset);
            T rsp = null;
            IZZXParser<T> parser = null;

            //如果验签失败则抛出异常
            RSAUtil.VerifySign(body, bizResponse, _publicKey, _charset);

            parser = new ZZXJsonParser<T>();
            rsp = parser.ParseBizObj(bizResponse, _charset);
            return rsp;
        }


        private ZZXDictionary getSystemParams<T>(IZZXRequest<T> request) where T : ZZXResponse
        {
            string apiVersion = Version;
            String appParamsQuery = WebUtils.BuildQuery(request.GetParameters(), _charset);
            string encryptedAppParam = RSAUtil.Encrypt(appParamsQuery, _publicKey, _charset);
            ZZXDictionary sysParams = new ZZXDictionary();
            sysParams.Add(METHOD, request.GetApiName());
            sysParams.Add(VERSION, apiVersion);
            sysParams.Add(ChANNELID,_channelId);
            sysParams.Add(SIGNTYPE,"RSA2");//写死
            // 添加签名参数
            sysParams.Add(SIGN, RSAUtil.Sign(appParamsQuery, _privateKey, _charset));

            sysParams.Add(PARAMS, encryptedAppParam);
            sysParams.Add(CHARSET, _charset);

            return sysParams;
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
