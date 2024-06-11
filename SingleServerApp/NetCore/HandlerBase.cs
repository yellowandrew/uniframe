using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore
{
    public abstract class HandlerBase
    {
        public int TAG;
        public MessageCenter center;
        public HandlerBase(int tag)
        {
            this.TAG = tag;
        }

        public abstract void OnMessage(Connection connection, SocketMSG message);
        public abstract void OnClientClose(Connection connection, string error);
        public virtual void OnClientConnect(Connection connection) { }

        public void Brocast(int opCode, int subCode, int command, object message, Connection connection = null)
        {
            center.Brocast(opCode, subCode, command, message, connection);
        }

        public void Send(Connection connection, int opCode, int subCode, int command, object message)
        {
            center.Send(connection, opCode, subCode, command, message);
        }
    }
}
