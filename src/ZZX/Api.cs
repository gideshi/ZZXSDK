using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZZX
{
    public static class Api
    {
        //public static string url { get; } = "https://ssl-scf.xingyoucai.com/api/v1/ickey/entry.do";//测试地址

        #region  3rd=>zzx

        /// <summary>
        /// 注册新客户
        /// </summary>
        public static string registerNewCustomer { get; } ="registerNewCustomer";

        /// <summary>
        /// 上传资料
        /// </summary>
        public static string uploadAttachment { get; } = "uploadAttachment";

        /// <summary>
        /// 申请贷款
        /// </summary>
        public static string loanApply { get; } = "loanApply";

        /// <summary>
        /// 确认贷款方案
        /// </summary>
        public static string loanApplySubmit { get; } = "loanApplySubmit";

        /// <summary>
        /// 贷款申请结果确认
        /// </summary>
        public static string loanContractConfirm { get; } = "loanContractConfirm";

        /// <summary>
        /// 主动还款通知
        /// </summary>
        public static string refundNotify { get; } = "refundNotify";

        /// <summary>
        /// 发票信息维护
        /// </summary>
        public static string upsertTicket { get; } = "upsertTicket";

        #endregion

        #region  zzx=>3rd

        /// <summary>
        /// 贷款申请结果通知
        /// </summary>
        public static string loanApplyResultNotify { get; } = "loanApplyResultNotify";

        /// <summary>
        /// 贷款放款完成通知
        /// </summary>
        public static string paymentNotify { get; } = "paymentNotify";

        /// <summary>
        /// 还款结果通知
        /// </summary>
        public static string refundResultNotify { get; } = "refundResultNotify";

        /// <summary>
        /// 客户还款提醒
        /// </summary>
        public static string customerRefundNotify { get; } = "customerRefundNotify";

        /// <summary>
        /// 获取用户订单历史
        /// </summary>
        public static string getOrderHistory { get; } = "getOrderHistory";
        
        #endregion
    }
}
