using LiteNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginHandler : Handler
{
    public LoginHandler()
    {

    }
    protected override void HandleUnKownPackage(object data, uint id)
    {

    }

    [PackageHandle(2)]
    public void OnLogin(NetConnection connection, Package package)
    {
       
        var pk = package as LoginResponePackage;
        Debug.Log("Fight Respone :" + pk.msg);

    }
}

public class FightHandler : Handler
{
    public FightHandler()
    {

    }

    [PackageHandle(20)]
    public void OnFight(NetConnection connection, Package package)
    {
        var pk = package as FightResponePackage;
        Debug.Log("Fight Respone :"+pk.msg);

    }
}
