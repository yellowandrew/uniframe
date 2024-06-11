using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LiteNetwork
{
    public interface IParser {
        byte[] WritePackageToBuffer(Package package);
         Package ReadPackageFromBuffer(byte[] buf);
    }
    public class Parser : IParser
    {
        Dictionary<UInt32, Type> packagetypes = new Dictionary<UInt32, Type>();
        public Parser()
        {
            GetPackage(GetType().Assembly);
            GetPackage(Assembly.GetEntryAssembly());

            foreach (var item in packagetypes.Values)
            {
                Console.WriteLine($"Package: {item.Name }");
            }
            
        }
        void GetPackage(Assembly asm)
        {
            var packageClasses = asm.GetTypes().Where(x => x.IsSubclassOf(typeof(Package)));
            foreach (var cs in packageClasses)
            {
                var attr = cs.GetCustomAttributes(typeof(PackageTypeAttribute), false);
                if (attr.FirstOrDefault() is PackageTypeAttribute packageTypeAttribute)
                {
                    packagetypes.Add(packageTypeAttribute.id, cs);
                }
            }
        }

        public Package ReadPackageFromBuffer(byte[] buf) {
            using (MemoryStream ms = new MemoryStream(buf)) {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    var packageType = br.ReadUInt32();
                    if (packagetypes.TryGetValue(packageType, out var type))
                    {
                        var package = Activator.CreateInstance(type) as Package ;
                        package.Decode(br);
                        Console.WriteLine($"Read Package: {package.GetType()}");
                        return package;
                    }
                }
            }
            throw new InvalidOperationException("Unkown Package !!");

        }

        public byte[] WritePackageToBuffer(Package package)
        {
            Console.WriteLine($"Write Package: {package.GetType()}");
            using (MemoryStream ms = new MemoryStream()) {
                using (BinaryWriter bw = new BinaryWriter(ms)) {
                    package.Encode(bw);
                    bw.Flush();
                    byte[] byteArray = new byte[(int)ms.Length];
                   System.Buffer.BlockCopy(ms.GetBuffer(), 0, byteArray, 0, (int)ms.Length);
                    return byteArray;
                }
            }
        }
       
    }

}
