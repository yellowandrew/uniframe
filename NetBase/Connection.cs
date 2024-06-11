using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetBase
{
    public class Connection
    {
        public NetServer server;
        public Socket socket;
        //用户异步接收网络数据对象
        public SocketAsyncEventArgs receiveEventArgs;
        //用户异步发送网络数据对象
        public SocketAsyncEventArgs sendEventArgs;
        List<byte> cache = new List<byte>();

        private bool isReading = false;
        private bool isWriting = false;

        /// <summary>
        /// 发送的消息的一个队列
        /// </summary>
        private Queue<byte[]> writeQueue = new Queue<byte[]>();

        public IDecodetor coder;
        public Connection()
        {
            receiveEventArgs = new SocketAsyncEventArgs();
            sendEventArgs = new SocketAsyncEventArgs();

            receiveEventArgs.UserToken = this;
            sendEventArgs.UserToken = this;

            //设置接收对象的缓冲区大小
            receiveEventArgs.SetBuffer(new byte[1024], 0, 1024);
        }

        //网络消息到达
        public void receive(byte[] buff)
        {
            //将消息写入缓存
            cache.AddRange(buff);
            if (!isReading)
            {
                isReading = true;
                onData();
            }
        }
        //缓存中有数据处理
        void onData()
        {
            //解码消息存储对象
            byte[] buff = coder.decode_packet(ref cache);
            //消息未接收全 退出数据处理 等待下次消息到达
            if (buff == null) { isReading = false; return; }

            //进行消息反序列化
            object message = coder.decode_msg(buff);
            //TODO 通知应用层 有消息到达
            server.OnMessageReceive(this, message);
            //尾递归
            onData();
        }
        ////粘包拆包问题 ： 解决决策 ：消息头和消息尾。
        //// 比如 发送的数据：  12345
        //void test()
        //{
        //    byte[] bt = Encoding.Default.GetBytes("12345");

        //    //怎么构造
        //    //头：消息的长度 bt.Length
        //    //尾：具体的消息 bt
        //    int length = bt.Length;
        //    byte[] bt1 = BitConverter.GetBytes(length);
        //    //得到消息就是：bt1 + bt

        //    ///怎么读取
        //    //int length = 前四个字节转成int类型
        //    //然后读取 这个长度的数据
        //}

        public void write(byte[] packet)
        {
            if (socket == null)
            {
                //此连接已经断开了
               server.ClientClose(this, "调用已经断开的连接");
                return;
            }
            writeQueue.Enqueue(packet);
            if (!isWriting)
            {
                isWriting = true;
                onWrite();
            }
        }

        public void onWrite()
        {
            //判断发送消息队列是否有消息
            if (writeQueue.Count == 0) { isWriting = false; return; }
            //取出第一条待发消息
            byte[] buff = writeQueue.Dequeue();
            //设置消息发送异步对象的发送数据缓冲区数据
            sendEventArgs.SetBuffer(buff, 0, buff.Length);
            //开启异步发送
            bool result = socket.SendAsync(sendEventArgs);
            //是否挂起
            if (!result)
            {
                server.ProcessSend(sendEventArgs);
            }
        }
        public void writed()
        {
            //与onData尾递归同理
            onWrite();
        }
        public void Close()
        {
            try
            {
                writeQueue.Clear();
                cache.Clear();
                isReading = false;
                isWriting = false;
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                socket = null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void Print() {
            
        }
    }
}
