using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZZX.Entities
{
    public interface IBaseRequest
    {
        string Method { get; set; }
        string Ver { get;}
        string ChannelId { get; set; }
        string SignType { get; }
        string Sign { get; set; }
        string Params { get; set; }
    }
    public class BaseRequest:IBaseRequest
    {
        public string Method { get; set; }
        public string Ver { get;} = "v1";
        public string ChannelId { get; set; }
        public string SignType { get; } = "RSA2";
        public string Sign { get; set; }
        public string Params { get; set; }

        //默认把参数签名增加到参数中

    }
}
