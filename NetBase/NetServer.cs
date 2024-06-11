using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetBase
{
    public class NetServer
    {
        Dictionary<int, Handler> handlers = new Dictionary<int, Handler>();
        List<Connection> connections = new List<Connection>();
        Socket listenSocket;
        Semaphore acceptedClients; // 信号量
        ConnectionPool connectionPool;
        int port;
        int maxConnection;
        int preload = 30;
        int online = 0;
        IDecodetor decodetor;
        public NetServer() {
            LoadCfg();
            Init();
        }
        private void LoadCfg()
        {
            port = int.Parse(ConfigurationManager.AppSettings["port"]);
            preload = int.Parse(ConfigurationManager.AppSettings["preload"]);
            maxConnection = int.Parse(ConfigurationManager.AppSettings["maxConnection"]);
        }
        private void Init()
        {
            SetupDecodetor();
            SetupHandlers();
           
            connectionPool = new ConnectionPool(maxConnection);
            acceptedClients = new Semaphore(maxConnection, maxConnection);
            for (int i = 0; i < preload; i++)
            {
                NewConnection();
            }

        }

        void LoadAttribute<T>(Action action)where T:Attribute {
            Assembly asm = Assembly.GetEntryAssembly();
            Type[] types = asm.GetExportedTypes();
            foreach (var t in types)
            {
                if (t.IsClass)
                {
                    T attr = t.GetCustomAttribute(typeof(T), false) as T;
                    if (attr != null)
                    {
                        action?.Invoke();
                    }
                }
            }

        }
        void SetupDecodetor() {
            Assembly asm = Assembly.GetEntryAssembly();
            Type[] types = asm.GetExportedTypes();
            foreach (var t in types) {
                if (t.IsClass)
                {
                    DecodeAttribute attr = t.GetCustomAttribute(typeof(DecodeAttribute), false) as DecodeAttribute;
                    if (attr != null)
                    {
                        decodetor = Activator.CreateInstance(t, new object[] {  }) as IDecodetor;
                    }
                }
            }
            
            if (decodetor == null)
            {
                decodetor = new DefaultDecoder();
            }
        }
        void SetupHandlers()
        {
            Assembly asm = Assembly.GetEntryAssembly();
            Type[] types = asm.GetExportedTypes();
            foreach (var t in types)
            {
                if (t.IsClass)
                {
                    HandlerAttribute attr = t.GetCustomAttribute(typeof(HandlerAttribute), false) as HandlerAttribute;
                    if (attr != null)
                    {
                        Handler h = Activator.CreateInstance(t, new object[] { attr.type }) as Handler;
                        h.server = this;
                        handlers.Add(attr.type, h);
                    }
                }
            }

        }
        void NewConnection()
        {
            Connection connection = new Connection();

            connection.receiveEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(IOComleted);
            connection.sendEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(IOComleted);
            connection.server = this;
            connection.coder = decodetor;
            connectionPool.push(connection);
        }

        private void IOComleted(object sender, SocketAsyncEventArgs e)
        {
            if (e.LastOperation == SocketAsyncOperation.Receive)
            {
                ProcessReceive(e);
            }
            else
            {
                ProcessSend(e);
            }
        }

        public void ProcessSend(SocketAsyncEventArgs e)
        {
            Connection connection = e.UserToken as Connection;
            if (e.SocketError != SocketError.Success)
            {
                ClientClose(connection, e.SocketError.ToString());
            }
            else
            {
                //消息发送成功，回调成功
                connection.writed();
            }
        }

        public void Start() {
            listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Console.WriteLine("服务器启动");
            try
            {
                listenSocket.Bind(new IPEndPoint(IPAddress.Any, port));
                Console.WriteLine("服务器监听端口:" + port);
                listenSocket.Listen(10);
                StartAccept(null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            wait();
        }
        /// <summary>
        /// 从客户端开始接受一个连接操作
        /// </summary>
        private void StartAccept(SocketAsyncEventArgs e)
        {
            //如果当前传入为空  说明调用新的客户端连接监听事件 否则的话 移除当前客户端连接
            if (e == null)
            {
                e = new SocketAsyncEventArgs();
                e.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
            }
            else
            {
                e.AcceptSocket = null;
            }

            //信号量-1
            acceptedClients.WaitOne();
            bool result = listenSocket.AcceptAsync(e);
            //判断异步事件是否挂起  没挂起说明立刻执行完成  直接处理事件 
            if (!result)
            {
                ProcessAccept(e);
            }
            //否则会在处理完成后触发OnAcceptCompleted事件
        }

        private void ProcessAccept(SocketAsyncEventArgs e)
        {
            online++;
            if (online > maxConnection)
            {
                //
            }
            if (online > preload)
            {
                NewConnection();
            }
            Connection connection = connectionPool.pop();
            connection.socket = e.AcceptSocket;
            // 有客户端连接
            OnClientConnect(connection);
            connections.Add(connection);
            //开启消息到达监听
            StartReceive(connection);
            //释放当前异步对象
            StartAccept(e);
        }

        private void StartReceive(Connection connection)
        {
            try
            {
                //用户连接对象 开启异步数据接收
                bool result = connection.socket.ReceiveAsync(connection.receiveEventArgs);
                //异步事件是否挂起
                if (!result)
                {
                    ProcessReceive(connection.receiveEventArgs);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            Connection connection = e.UserToken as Connection;
            //判断网络消息接收是否成功
            if (connection.receiveEventArgs.BytesTransferred > 0 && connection.receiveEventArgs.SocketError == SocketError.Success)
            {
                byte[] message = new byte[connection.receiveEventArgs.BytesTransferred];
                //将网络消息拷贝到自定义数组
                Buffer.BlockCopy(connection.receiveEventArgs.Buffer, 0, message, 0, connection.receiveEventArgs.BytesTransferred);
                //处理接收到的消息
                connection.receive(message);
                StartReceive(connection);
            }
            else
            {
                if (connection.receiveEventArgs.SocketError != SocketError.Success)
                {
                    ClientClose(connection, connection.receiveEventArgs.SocketError.ToString());
                }
                else
                {
                    ClientClose(connection, "客户端主动断开连接");
                }
            }
        }
        /// <summary>
        /// 客户端断开连接
        /// </summary>
        /// <param name="token"> 断开连接的用户对象</param>
        /// <param name="error">断开连接的错误编码</param>
        public void ClientClose(Connection connection, string error)
        {
            if (connection.socket != null)
            {
                lock (connection)
                {
                    //通知应用层面 客户端断开连接了
                    OnClientClose(connection, error);
                    connection.Close();
                    online--;
                    //加回一个信号量，供其它用户使用
                    connectionPool.push(connection);
                    connections.Remove(connection);
                    acceptedClients.Release();
                }
            }
        }

        private void OnClientConnect(Connection connection)
        {
            Console.WriteLine("有客户端连接了");
            connection.Print();
        }
        private void OnClientClose(Connection connection, string error)
        {
            foreach (var h in handlers.Values)
            {
                h.OnClientClose(connection, error);
            }
        }
        /// <summary>
        /// accept 操作完成时回调函数
        /// </summary>
        /// <param name="sender">Object who raised the event.</param>
        /// <param name="e">SocketAsyncEventArg associated with the completed accept operation.</param>
        private void OnAcceptCompleted(object sender, SocketAsyncEventArgs e)
        {
            ProcessAccept(e);
        }
        public void OnMessageReceive(Connection connection, object message)
        {
            NetMSG MSG = message as NetMSG;
            handlers[MSG.type].OnMessage(connection, MSG);
        }
        public void SendToClient(Connection connection, NetMSG message)
        {
            byte[] value = decodetor.encode_msg(message);
            value = decodetor.encode_packet(value);
            connection.write(value);
        }
        public void Brocast(NetMSG message, Connection connection = null) {
            byte[] value = decodetor.encode_msg(message);
            value = decodetor.encode_packet(value);
            foreach (Connection conn in connections)
            {
                if (conn != connection)
                {
                    byte[] bs = new byte[value.Length];
                    Array.Copy(value, 0, bs, 0, value.Length);
                    conn.write(bs);
                }
            }
        }

        void wait() {
            while (true)
            {
                var key = Console.ReadKey().Key;
                if (key == ConsoleKey.Escape) break;
            }
        }
    }
}
