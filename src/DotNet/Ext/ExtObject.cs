using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class ExtObject
    {
        public static string GetString(this object s)
        {
            if (object.Equals(s, null) || object.Equals(s, DBNull.Value))
            {
                return "";
            }
            return s.ToString().Trim();
        }

        
    }
}
