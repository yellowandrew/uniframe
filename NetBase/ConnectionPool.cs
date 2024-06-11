using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetBase
{
    public class ConnectionPool
    {
        private Stack<Connection> pool;
        public ConnectionPool(int count)
        {
            pool = new Stack<Connection>(count);
        }
        /// <summary>
        /// 取出一个连接（创建连接）
        /// </summary>
        public Connection pop()
        {
            return pool.Pop();
        }
        /// <summary>
        /// 放入一个连接（释放连接）
        /// </summary>
        public void push(Connection connection)
        {
            if (connection != null)
            {
                pool.Push(connection);
            }

        }

        public int Size {
            get { return pool.Count(); }
        }
    }
}
