using System;
using System.Reflection;

namespace ZZX.Parser
{
    public class ZZXAttribute
    {
        public string ItemName { get; set; }
        public Type ItemType { get; set; }
        public string ListName { get; set; }
        public Type ListType { get; set; }
        public MethodInfo Method { get; set; }
    }
}
