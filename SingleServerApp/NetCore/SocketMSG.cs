using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore
{
   public class SocketMSG
    {
        /// <summary>
        /// 操作码
        /// </summary>
        public int OpCode { get; set; }

        /// <summary>
        /// 子操作
        /// </summary>
        public int SubCode { get; set; }
        /// <summary>
        ///  用于区分当前处理逻辑功能
        /// </summary>
        public int Command { get; set; }
        /// <summary>
        ///  消息体 当前需要处理的主体数据
        /// </summary>
        public object Message { get; set; }

        public SocketMSG()
        {

        }

        public SocketMSG(int opCode, int subCode, int cmd, object msg)
        {
            this.OpCode = opCode;
            this.SubCode = subCode;
            this.Command = cmd;
            this.Message = msg;
        }

        public void print()
        {
            Console.WriteLine(OpCode + "-" + SubCode + "-" + Command + "-" + Message);
        }
    }
}
