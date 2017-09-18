using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZZX
{
    public class ZZXConstants
    {
        //public const string RESPONSE_SUFFIX = "_response";
        //public const string BIZ_RESPONSE_SIGN = "biz_response_sign";
        //public const string ERROR_RESPONSE = "error_response";
        //public const string SIGN = "sign";
        //public const string SIGN_SOURCE = "signSource";
        //public const string SIGN_RESULT = "signResult";
        //public const string ENCRYPTED = "encrypted";
        /// <summary>
        /// 中子星默认的是RSA2 如果用作其他接口请修改成对应的算法名称
        /// </summary>
        public const string SIGNTYPE = "RSA2";
        public const string DEFAULT_CHARSET = "UTF-8";
    }
}
