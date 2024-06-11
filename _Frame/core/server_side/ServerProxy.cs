using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ServerProxy 
{
    NetServer netServer;

    public ServerProxy()
    {
        INIParser parser = new INIParser();
        parser.Open(Application.streamingAssetsPath + "/game.cfg");
        string server = parser.ReadValue("server", "class", "TelepathyServer");

        var cfg = new {
            port= parser.ReadValue("server", "port", 7777),
            maxclient = parser.ReadValue("server", "maxclient", 100),
            buffer = parser.ReadValue("server", "buffer", 4096),
            tick = parser.ReadValue("server", "tick", 100),
        };
       
        parser.Close();
        netServer = Parser.CreateClass<NetServer>(server);
        netServer.LoadConfig(cfg);
    }

    public void Start() { netServer.Start(); }

    public void Poll() { netServer.Poll(); } 

}

