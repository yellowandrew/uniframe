using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LiteNetwork
{
    public abstract class Package
    {
            public UInt32 id;
            public Package(UInt32 id) { this.id = id; }
            public abstract void Encode(BinaryWriter writer);
            public abstract void Decode(BinaryReader reader);
        }
}
