using LiteNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetorkDemo
{
    public class LoginHandler : Handler
    {
        public LoginHandler() 
        {

        }
        protected override void HandleUnKownPackage(object data, uint id)
        {

        }

        [PackageHandle(1)]
        public void OnLogin(Connection connection,Package package, IParser parser)
        {
            var req = package as LoginRequestPackage;
            Console.WriteLine($"OnLogin Request->username:{req.Username} password:{req.Password}");

            var res = new LoginResponePackage() { msg = "登录成功" };
            connection.SendAsync(parser.WritePackageToBuffer(res));
        }
    }

    public class FightHandler : Handler
    {
        public FightHandler() 
        {

        }

        [PackageHandle(10)]
        public void OnFight(Connection connection, Package package,IParser parser)
        {
            
            var req = package as FightRequestPackage;
            Console.WriteLine($"OnFight Request->{req.Map}");
            var res = new FightResponePackage() { msg = "哈哈,开始战斗了!" };
            connection.SendAsync(parser.WritePackageToBuffer(res));

        }
    }

}
