using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZZX.Entities
{
    public enum RefundType
    {
        到期还款 = 1,
        提前还款 = 2,
        逾期还款 = 3
    }

    public enum RefundFlag
    {
        已还完完毕=1,
        未还款完毕=2
    }
}
