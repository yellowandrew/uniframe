using System;
using System.Collections.Generic;
using System.Text;

namespace LiteNetwork
{
    public abstract class Handler
    {
       public Handler()
        {
            
        }
        protected virtual void HandleUnKownPackage(object data, UInt32 id) {
            Console.WriteLine($"UnKown  Package ......");
        }

    }
}
