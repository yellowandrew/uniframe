using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore
{
    public class MessageCenter
    {

        Dictionary<int, HandlerBase> handlers;
        public ServerBase server;
        public MessageCenter()
        {
            handlers = new Dictionary<int, HandlerBase>();
        }
        public void AddHandler(HandlerBase handler)
        {
            handler.center = this;
            handlers.Add(handler.TAG, handler);
        }

        public void OnMessageReceive(Connection connection, object message)
        {
            SocketMSG MSG = message as SocketMSG;
            handlers[MSG.OpCode].OnMessage(connection, MSG);
        }

        public void OnClientConnect(Connection connection)
        {
            Console.WriteLine("有客户端连接了");
        }
        public void OnClientClose(Connection connection, string error)
        {
            foreach (var h in handlers.Values)
            {
                h.OnClientClose(connection, error);
            }
        }

        public void Brocast(int opCode, int subCode, int command, object message, Connection connection = null)
        {
            server.Brocast(opCode, subCode, command, message, connection);
        }
        public void Send(Connection connection, int opCode, int subCode, int command, object message)
        {
            server.SendToClient(connection, opCode, subCode, command, message);
        }
    }

}
