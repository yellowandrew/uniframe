using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;

namespace LiteNetwork
{

   public class Connection : TcpSession {
        public Connection(TcpServer server) : base(server) {
           
        }
        protected override void OnConnected()
        {
            Console.WriteLine($"Connection session with Id {Id} connected!");
        }
        protected override void OnDisconnected()
        {
            Console.WriteLine($"Connection session with Id {Id} disconnected!");
        }
        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            ((LiteTcpServer)Server).Dispatch(this, buffer);
        }
        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Connection session caught an error with code {error}");
        }
       
        public void Brocast(byte[] buffer) {
            Server.Multicast(buffer);
        }
    }
    public class LiteTcpServer : TcpServer
    {
        IParser parser;
        Dictionary<UInt32, Tuple<MethodInfo, Handler>> handleActions
            = new Dictionary<UInt32, Tuple<MethodInfo, Handler>>();

        public LiteTcpServer(IParser parser, IPAddress address, int port) : base(address, port)
        {
          
            this.parser = parser;
            var handleClasses = Assembly.GetEntryAssembly().GetTypes().Where(x => x.IsSubclassOf(typeof(Handler)));
            foreach (var cs in handleClasses)
            {
                var handler = Activator.CreateInstance(cs) as Handler;

                foreach (var meth in handler.GetType().GetMethods())
                {
                    var attrib = meth.GetCustomAttribute<PackageHandleAttribute>();
                    if (attrib == null) continue;
                    handleActions.Add(attrib.id, new Tuple<MethodInfo, Handler>(meth, handler));
                }
            }

            
        }

        public void Dispatch(Connection connection, byte[] buffer) {
            var package = parser.ReadPackageFromBuffer(buffer);
            if (handleActions.TryGetValue(package.id, out var item))
            {
                var (m, h) = item;
                m.Invoke(h, new object[] { connection, package,parser });
            }

        }
        protected override TcpSession CreateSession() => new Connection(this);

        protected override void OnStarting()
        {
            base.OnStarting();
            Console.WriteLine($"Server OnStarting......");
        }
        protected override void OnStarted()
        {
            base.OnStarted();
            Console.WriteLine($"Server OnStarted!!");
        }

        protected override void OnConnecting(TcpSession session)
        {
            base.OnConnecting(session);
            Console.WriteLine($"Connection:{session.Id} OnConnecting......");
        }
        protected override void OnConnected(TcpSession session)
        {
            base.OnConnected(session);
            Console.WriteLine($"Connection:{session.Id} OnConnected!!");
        }

        protected override void OnDisconnecting(TcpSession session)
        {
            base.OnDisconnecting(session);
            Console.WriteLine($"Connection:{session.Id} OnDisconnecting......");
        }

        protected override void OnDisconnected(TcpSession session)
        {
            base.OnDisconnected(session);
            Console.WriteLine($"Connection:{session.Id} OnDisconnected!!");
        }

        protected override void OnStopping()
        {
            base.OnStopping();
            Console.WriteLine($"Server OnStopping......");
        }

        protected override void OnStopped()
        {
            base.OnStopped();
            Console.WriteLine($"Server OnStopped!!");
        }
        protected override void OnError(SocketError error)
        {
            Console.WriteLine($"Server caught an error with code {error}");
        }
    }
}
