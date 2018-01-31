using System;
using System.Collections.Generic;
using System.Text;

namespace Messages
{
    /// <summary>
    /// 基本消息
    /// </summary>
    public struct ResultInfo
    {
        /// <summary>
        /// 错误码 0表示成功,其它表示失败
        /// </summary>
        public int ErrorCode { get; set; }
        /// <summary>
        /// 失败信息
        /// </summary>
        public string ErrorInfo { get; set; }
    }
}
