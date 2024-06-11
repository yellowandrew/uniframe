using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteNetwork;
public class NetScript : MonoBehaviour
{

    NetConnection net;
    void Start()
    {
        net = new NetConnection(new Parser(), "127.0.0.1", 5678);
        net.ConnectAsync();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            LoginRequestPackage login = new LoginRequestPackage();
            login.Username = "And";
            login.Password = "123qwe";
            net.SendPackageAsyn(login);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            FightRequestPackage fight = new FightRequestPackage();
            fight.Map = "Sun";
          
            net.SendPackageAsyn(fight);
        }
    }
}
