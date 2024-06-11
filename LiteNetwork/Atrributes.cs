using System;
using System.Collections.Generic;
using System.Text;

namespace LiteNetwork
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class PackageTypeAttribute : Attribute
    {
        public UInt32 id;
        public PackageTypeAttribute(UInt32 id)
        {
            this.id = id;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class PackageHandleAttribute : Attribute
    {
        public UInt32 id { get; }

        public PackageHandleAttribute(UInt32 id)
        {
            this.id = id;
        }
    }

}
