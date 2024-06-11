using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace NetCore
{
    public class DefaultEncoder
    {
        #region 粘包拆包问题
        public virtual byte[] encode_packet(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    //先写入长度
                    bw.Write(data.Length);
                    //再写入数据
                    bw.Write(data);

                    byte[] byteArray = new byte[(int)ms.Length];
                    Buffer.BlockCopy(ms.GetBuffer(), 0, byteArray, 0, (int)ms.Length);

                    return byteArray;
                }
            }
        }
        public virtual byte[] decode_packet(ref List<byte> dataCache)
        {
            //四个字节 构成一个int长度 不能构成一个完整的消息
            if (dataCache.Count < 4)
                return null;
            //throw new Exception("数据缓存长度不足4 不能构成一个完整的消息");

            using (MemoryStream ms = new MemoryStream(dataCache.ToArray()))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    // 1111 111 1
                    int length = br.ReadInt32();
                    int dataRemainLength = (int)(ms.Length - ms.Position);
                    //数据长度不够包头约定的长度 不能构成一个完整的消息
                    if (length > dataRemainLength)
                        return null;
                    //throw new Exception("数据长度不够包头约定的长度 不能构成一个完整的消息");

                    byte[] data = br.ReadBytes(length);
                    //更新一下数据缓存
                    dataCache.Clear();
                    dataCache.AddRange(br.ReadBytes(dataRemainLength));

                    return data;
                }
            }
        }

        #endregion
        /// 对象序列化
        public virtual byte[] encode_obj(object data)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, data);
                byte[] result = new byte[ms.Length];
                Buffer.BlockCopy(ms.GetBuffer(), 0, result, 0, (int)ms.Length);
                return result;
            }
        }
        public virtual object decode_obj(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                BinaryFormatter bf = new BinaryFormatter();
                object result = bf.Deserialize(ms);
                return result;
            }
        }
        /// 消息体序列化
        public virtual byte[] encode_msg(object data)
        {
            SocketMSG msg = data as SocketMSG;
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write(msg.OpCode);
            bw.Write(msg.SubCode);
            bw.Write(msg.Command);
            //如果不等于null  才需要把object 转换成字节数据 存起来
            if (msg.Message != null)
            {
                byte[] valueBytes = encode_obj(msg.Message);
                bw.Write(valueBytes);
            }
            byte[] result = new byte[ms.Length];
            Buffer.BlockCopy(ms.GetBuffer(), 0, result, 0, (int)ms.Length);
            bw.Close();
            ms.Close();
            return result;
        }

        public virtual object decode_msg(byte[] data)
        {
            SocketMSG msg = new SocketMSG();
            MemoryStream ms = new MemoryStream(data);
            BinaryReader br = new BinaryReader(ms);

            msg.OpCode = br.ReadInt32();
            msg.SubCode = br.ReadInt32();
            msg.Command = br.ReadInt32();
            //还有剩余的字节没读取 代表 message  有值
            if (ms.Length > ms.Position)
            {
                byte[] valueBytes = br.ReadBytes((int)(ms.Length - ms.Position));
                object value = decode_obj(valueBytes);
                msg.Message = value;
            }
            br.Close();
            ms.Close();
            return msg;
        }
    }

}
