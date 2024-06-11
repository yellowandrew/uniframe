using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetBase
{
    public class HandlerAttribute : Attribute
    {
        public int type;
        public HandlerAttribute(int type)
        {
            this.type = type;
        }
    }

    public class DecodeAttribute : Attribute { }
}
