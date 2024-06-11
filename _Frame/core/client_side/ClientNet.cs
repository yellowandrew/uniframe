using System;
using System.Collections;
using System.Collections.Generic;
using Telepathy;
using UnityEngine;

public class ClientNet 
{
    protected Game game;
    protected string ip = "127.0.0.1";
    protected int port = 7777;
    protected int size = 4096;
    protected int tick = 100;
 
    public ClientNet()
    {
        INIParser parser = new INIParser();
        parser.Open(Application.streamingAssetsPath + "/game.cfg");
        ip = parser.ReadValue("server", "ip", ip);
        port = parser.ReadValue("server", "port", port);
        tick = parser.ReadValue("server", "tick", tick);
        size = parser.ReadValue("buffer", "size", size);
        parser.Close();
    }
    public void SetGame(Game game) { this.game = game; }
    public virtual void Connect() { }
    public virtual void Poll() { }
    public virtual void Send(object data) { }
}
