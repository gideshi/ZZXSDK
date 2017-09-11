using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZZX.Util;

namespace ZZX
{
    interface IZZXUploadRequest<T> : IZZXRequest<T> where T : ZZXResponse
    {
        /// <summary>
        /// 获取所有的Key-Value形式的文件请求参数字典。其中：
        /// Key: 请求参数名
        /// Value: 文件对象
        /// </summary>
        /// <returns>文件请求参数字典</returns>
        IDictionary<string, FileItem> GetFileParameters();
    }
}
