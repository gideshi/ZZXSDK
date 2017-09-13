using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZZX
{
    public interface IZZXRequest<T> where T : ZZXResponse
    {

        /// <summary>
        /// Api方法名称
        /// </summary>
        /// <returns></returns>
        string GetApiName();

        string GetParams();

        /// <summary>
        /// 获取所有的Key-Value形式的文本请求参数字典。其中：
        /// Key: 请求参数名
        /// Value: 请求参数文本值
        /// </summary>
        /// <returns>文本请求参数字典</returns>
        IDictionary<string, string> GetParameters();
    }
}
