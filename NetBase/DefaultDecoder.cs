using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace NetBase
{

   
    public class DefaultDecoder : IDecodetor
    {
     
        public byte[] encode_msg(object data)
        {
            NetMSG msg = data as NetMSG;
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write(msg.type);
            bw.Write(msg.code);
            bw.Write(msg.cmd);
            //如果不等于null  才需要把object 转换成字节数据 存起来
            if (msg.msg != null)
            {
                byte[] valueBytes = encode_obj(msg.msg);
                bw.Write(valueBytes);
            }
            byte[] result = new byte[ms.Length];
            Buffer.BlockCopy(ms.GetBuffer(), 0, result, 0, (int)ms.Length);
            bw.Close();
            ms.Close();
            return result;
        }
        public object decode_msg(byte[] data)
        {
            NetMSG msg = new NetMSG();
            MemoryStream ms = new MemoryStream(data);
            BinaryReader br = new BinaryReader(ms);

            msg.type = br.ReadInt32();
            msg.code = br.ReadInt32();
            msg.cmd = br.ReadInt32();
            //还有剩余的字节没读取 代表 message  有值
            if (ms.Length > ms.Position)
            {
                byte[] valueBytes = br.ReadBytes((int)(ms.Length - ms.Position));
                object value = decode_obj(valueBytes);
                msg.msg = value;
            }
            br.Close();
            ms.Close();
            return msg;
        }
        public byte[] encode_obj(object data)
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

        public object decode_obj(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                BinaryFormatter bf = new BinaryFormatter();
                object result = bf.Deserialize(ms);
                return result;
            }
        }

        public byte[] decode_packet(ref List<byte> cache)
        {
            //四个字节 构成一个int长度 不能构成一个完整的消息
            if (cache.Count < 4)
                return null;
            //throw new Exception("数据缓存长度不足4 不能构成一个完整的消息");

            using (MemoryStream ms = new MemoryStream(cache.ToArray()))
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
                   cache.Clear();
                    cache.AddRange(br.ReadBytes(dataRemainLength));

                    return data;
                }
            }
        }
        
        public byte[] encode_packet(byte[] data)
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
    }
}
