using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetBase
{
    public class NetMSG
    {
        /// <summary>
        /// 消息类型
        /// </summary>
        public int type { get; set; }

        /// <summary>
        /// 操作码
        /// </summary>
        public int code { get; set; }
        /// <summary>
        ///  指令逻辑
        /// </summary>
        public int cmd { get; set; }
        /// <summary>
        ///  消息体 当前需要处理的主体数据
        /// </summary>
        public object msg { get; set; }

        public NetMSG()
        {

        }

        public NetMSG(int type, int code, int cmd, object msg)
        {
            this.type = type;
            this.code = code;
            this.cmd = cmd;
            this.msg = msg;
        }
        public T GetMSG<T>()
        {
            return (T)msg;
        }
        public void Print()
        {
            Console.WriteLine(type + "-" + code + "-" + cmd + "-" + msg);
        }
    }
}
