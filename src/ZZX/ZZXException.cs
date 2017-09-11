using System;
using System.Runtime.Serialization;

namespace ZZX
{
    /// <summary>
    /// ZMOP客户端异常。
    /// </summary>
    public class ZZXException : Exception
    {
        private string errorCode;
        private string errorMsg;

        public ZZXException()
            : base()
        {
        }

        public ZZXException(string message)
            : base(message)
        {
        }

        protected ZZXException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public ZZXException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public ZZXException(string errorCode, string errorMsg)
            : base(errorCode + ":" + errorMsg)
        {
            this.errorCode = errorCode;
            this.errorMsg = errorMsg;
        }

        public string ErrorCode
        {
            get { return this.errorCode; }
        }

        public string ErrorMsg
        {
            get { return this.errorMsg; }
        }
    }
}
