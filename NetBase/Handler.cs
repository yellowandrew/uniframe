using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetBase
{
    public abstract class Handler
    {
        protected int type;
        public NetServer server;
        public Handler(int type) {
            this.type = type;
        }

        public abstract void OnMessage(Connection connection, NetMSG message);
        public abstract void OnClientClose(Connection connection, string error);
        public virtual void OnClientConnect(Connection connection) { }

        public void BrocastMessage(NetMSG message, Connection connection = null) {
            server.Brocast(message, connection);
        }
        public void SendMessage(Connection connection, NetMSG message) {
            server.SendToClient(connection, message);
        }
    }
}
