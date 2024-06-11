using NetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApp
{
    public class DefaultHandler : HandlerBase
    {
        public DefaultHandler(int tag) : base(tag) { }
        public override void OnClientClose(Connection connection, string error)
        {
            Console.WriteLine("客户端断开连接");
        }

        public override void OnMessage(Connection connection, SocketMSG message)
        {
            message.print();
            Send(connection, 1, 2, 3, "收到:"+message.Message);
        }
    }
}
