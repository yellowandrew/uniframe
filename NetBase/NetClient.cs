using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetBase
{
   public class NetClient
    {
        Socket socket;
        IDecodetor decodetor;
        //接受的数据缓冲区
        private byte[] receiveBuffer = new byte[1024];

        /// <summary>
        /// 一旦接收到数据 就存到缓存区里面
        /// </summary>
        private List<byte> dataCache = new List<byte>();
        private bool isProcessReceive = false;
        public Queue<NetMSG> NetMsgQueue = new Queue<NetMSG>();


        public NetClient() {
            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public NetClient(IDecodetor decodetor) : this()
        {
            this.decodetor = decodetor;
        }
        public void setDecodetor(IDecodetor decodetor) {
            this.decodetor = decodetor;
        }
        public void Connect(string ip,int port) {
            try
            {
                socket.Connect(ip, port);
                startReceive();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public void Close()
        {
            try
            {
                NetMsgQueue.Clear();
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                socket = null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        private void startReceive()
        {
            if (socket == null && socket.Connected == false)
            {
                Console.WriteLine("没有连接成功，无法发送数据");
                return;
            }

            socket.BeginReceive(receiveBuffer, 0, 1024, SocketFlags.None, receiveCallBack, socket);
        }

        /// <summary>
        /// 收到消息的回调
        /// </summary>
        private void receiveCallBack(IAsyncResult ar)
        {
            try
            {
                int length = socket.EndReceive(ar);
                byte[] tmpByteArray = new byte[length];
                Buffer.BlockCopy(receiveBuffer, 0, tmpByteArray, 0, length);

                //处理收到的数据
                dataCache.AddRange(tmpByteArray);
                if (isProcessReceive == false)
                    processReceive();

                startReceive();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        /// <summary>
        /// 处理收到的数据
        /// </summary>
        private void processReceive()
        {
            isProcessReceive = true;
            //解析数据包
            byte[] data = decodetor.decode_packet(ref dataCache);

            if (data == null)
            {
                isProcessReceive = false;
                return;
            }

            NetMSG msg = decodetor.decode_msg(data) as NetMSG;
            //存储消息 等待处理
            NetMsgQueue.Enqueue(msg);

            //尾递归
            processReceive();
        }

        public void SendMessage(NetMSG MSG) {
            ByteArray ba = new ByteArray();
            ba.write(MSG.type);
            ba.write(MSG.code);
            ba.write(MSG.cmd);
            //判断消息体是否为空  不为空则序列化后写入
            if (MSG.msg != null)
            {
                ba.write(decodetor.encode_obj(MSG.msg));
            }
            ByteArray arr1 = new ByteArray();
            arr1.write(ba.Length);
            arr1.write(ba.getBuff());
            try
            {
                socket.Send(arr1.getBuff());
            }
            catch (Exception e)
            {
                Console.WriteLine("网络错误，请重新登录" + e.Message);
            }
        }
    }
}
