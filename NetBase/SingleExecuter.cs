using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetBase
{
    /// <summary>
    /// 一个需要执行的方法
    /// </summary>
    public delegate void ExecuteDelegate();
    class SingleExecuter
    {
        private static SingleExecuter instance = null;

        /// <summary>
        /// 单例
        /// </summary>
        public static SingleExecuter Instance {
            get {
                lock (o)
                {
                    if (instance == null)
                        instance = new SingleExecuter();
                    return instance;
                }
            }
        }

        private static object o = 1;

        /// <summary>
        /// 互斥锁
        /// </summary>
        public Mutex mutex;

        private SingleExecuter()
        {
            mutex = new Mutex();
        }

        /// <summary>
        /// 单线程处理逻辑
        /// </summary>
        /// <param name="executeDelegate"></param>
        public void Execute(ExecuteDelegate executeDelegate)
        {
            lock (this)
            {
                mutex.WaitOne();
                executeDelegate();
                mutex.ReleaseMutex();
            }
        }

    }
}
