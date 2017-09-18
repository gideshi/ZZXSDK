using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ZZX
{
    [Serializable]
    public abstract class ZZXResponse
    {
        //private bool success;
        private int statusCode;
        private string errMsg;
        private string method;
        private string ver;
        private string channelId;
        private string signType;
        private string sign;
        private object parms;

        public int StatusCode
        {
            get { return statusCode; }
            set { statusCode = value; }
        }

        public string ErrMsg
        {
            get { return errMsg; }
            set { errMsg = value; }
        }

        public string Method
        {
            get { return method; }
            set { method = value; }
        }
        public string Ver
        {
            get { return ver; }
            set { ver = value; }
        }

        public string ChannelId
        {
            get { return channelId; }
            set { channelId = value; }
        }

        public string SignType
        {
            get { return signType; }
            set { signType = value; }
        }
        public string Sign
        {
            get { return sign; }
            set { sign = value; }
        }
        public object Params
        {
            get { return parms; }
            set { parms = value; }
        }
    }
}
