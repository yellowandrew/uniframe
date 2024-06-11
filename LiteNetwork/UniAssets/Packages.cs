using LiteNetwork;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    [PackageType(1)]
    public class LoginRequestPackage : Package
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public LoginRequestPackage() : base(1)
        {

        }

        public override void Encode(BinaryWriter writer)
        {
            writer.Write((UInt32)id);
            writer.Write(Username);
            writer.Write(Password);
        }
        public override void Decode(BinaryReader reader)
        {
            Username = reader.ReadString();
            Password = reader.ReadString();
        }
    }

    [PackageType(2)]
    public class LoginResponePackage : Package
    {

        public string msg { get; set; }

        public LoginResponePackage() : base(2)
        {

        }

        public override void Encode(BinaryWriter writer)
        {
            writer.Write((UInt32)id);
            writer.Write(msg);
        }

        public override void Decode(BinaryReader reader)
        {
            msg = reader.ReadString();
        }
    }

    [PackageType(10)]
    public class FightRequestPackage : Package
    {
        public string Map { get; set; }
        public FightRequestPackage() : base(10)
        {

        }

        public override void Encode(BinaryWriter writer)
        {
            writer.Write((UInt32)id);
            writer.Write(Map);
            
        }
        public override void Decode(BinaryReader reader)
        {
            Map = reader.ReadString();
           
        }
    }

    [PackageType(20)]
    public class FightResponePackage : Package
    {

        public string msg { get; set; }

        public FightResponePackage() : base(20)
        {

        }

        public override void Encode(BinaryWriter writer)
        {
            writer.Write((UInt32)id);
            writer.Write(msg);
        }

        public override void Decode(BinaryReader reader)
        {
            msg = reader.ReadString();
        }
    }

