using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class NetSocket
{
    private Socket socket;

    public NetSocket()
    {
        try
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
        }
    }
    public void Connect(string ip, int port)
    {
        try
        {
            socket.Connect(ip, port);
            Debug.Log("连接服务器成功！");
            startReceive();
        }
        catch (Exception e)
        {
            Debug.Log("无法连接服务器！");
            Debug.LogError(e.Message);
           
        }
    }
    public void Close()
    {
        try
        {
            SocketMsgQueue.Clear();
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
            socket = null;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    #region 接受数据

    //接受的数据缓冲区
    private byte[] receiveBuffer = new byte[1024];

    /// <summary>
    /// 一旦接收到数据 就存到缓存区里面
    /// </summary>
    private List<byte> dataCache = new List<byte>();

    private bool isProcessReceive = false;

    public Queue<SocketMSG> SocketMsgQueue = new Queue<SocketMSG>();

    /// <summary>
    /// 开始异步接受数据
    /// </summary>
    private void startReceive()
    {
        if (socket == null && socket.Connected == false)
        {
            Debug.LogError("没有连接成功，无法发送数据");
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
            Debug.LogError(e.Message);
        }
    }

    /// <summary>
    /// 处理收到的数据
    /// </summary>
    private void processReceive()
    {
        isProcessReceive = true;
        //解析数据包
        byte[] data = decode(ref dataCache);

        if (data == null)
        {
            isProcessReceive = false;
            return;
        }

        SocketMSG msg = mdecode(data);
        //存储消息 等待处理
        SocketMsgQueue.Enqueue(msg);

        //尾递归
        processReceive();
    }

    #endregion

    public void Send(SocketMSG MSG) {
        Send(MSG.OpCode, MSG.SubCode, MSG.Command, MSG.Message);
    }
    public void Send(int opCode, int subCode, int command, object message)
    {
        ByteArray ba = new ByteArray();
        ba.write(opCode);
        ba.write(subCode);
        ba.write(command);
        //判断消息体是否为空  不为空则序列化后写入
        if (message != null)
        {
            ba.write(encode_obj(message));
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
            Debug.Log("网络错误，请重新登录" + e.Message);
        }

    }
    public  byte[] decode(ref List<byte> cache)
    {
        if (cache.Count < 4) return null;

        MemoryStream ms = new MemoryStream(cache.ToArray());//创建内存流对象，并将缓存数据写入进去
        BinaryReader br = new BinaryReader(ms);//二进制读取流
        int length = br.ReadInt32();//从缓存中读取int型消息体长度
        //如果消息体长度 大于缓存中数据长度 说明消息没有读取完 等待下次消息到达后再次处理
        if (length > ms.Length - ms.Position)
        {
            return null;
        }
        //读取正确长度的数据
        byte[] result = br.ReadBytes(length);
        //清空缓存
        cache.Clear();
        //将读取后的剩余数据写入缓存
        cache.AddRange(br.ReadBytes((int)(ms.Length - ms.Position)));
        br.Close();
        ms.Close();
        return result;
    }

    public SocketMSG mdecode(byte[] value)
    {
        ByteArray ba = new ByteArray(value);
        SocketMSG model = new SocketMSG();
        int type;
        int area;
        int command;
        //从数据中读取 三层协议  读取数据顺序必须和写入顺序保持一致
        ba.read(out type);
        ba.read(out area);
        ba.read(out command);
        model.OpCode = type;
        model.SubCode = area;
        model.Command = command;
        //判断读取完协议后 是否还有数据需要读取 是则说明有消息体 进行消息体读取
        if (ba.Readnable)
        {
            byte[] message;
            //将剩余数据全部读取出来
            ba.read(out message, ba.Length - ba.Position);
            //反序列化剩余数据为消息体
            model.Message = decode_obj(message);
        }
        ba.Close();
        return model;
    }

      object decode_obj(byte[] value)
    {
        MemoryStream ms = new MemoryStream(value);//创建编码解码的内存流对象 并将需要反序列化的数据写入其中
        BinaryFormatter bw = new BinaryFormatter();//二进制流序列化对象
         //将流数据反序列化为obj对象
        object result = bw.Deserialize(ms);
        ms.Close();
        return result;
    }
    public  byte[] encode_obj(object value)
    {
        MemoryStream ms = new MemoryStream();//创建编码解码的内存流对象
        BinaryFormatter bw = new BinaryFormatter();//二进制流序列化对象
                                                   //将obj对象序列化成二进制数据 写入到 内存流
        bw.Serialize(ms, value);
        byte[] result = new byte[ms.Length];
        //将流数据 拷贝到结果数组
        Buffer.BlockCopy(ms.GetBuffer(), 0, result, 0, (int)ms.Length);
        ms.Close();
        return result;
    }
}
