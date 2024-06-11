using NetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApp
{
    public class SnakeServer : ServerBase
    {
        public override void SetHandlers()
        {
            AddHandler(new DefaultHandler(1));
        }
    }
}
