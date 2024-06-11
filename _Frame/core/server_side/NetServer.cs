using System;
using System.Collections;
using System.Collections.Generic;
using Telepathy;
using UnityEngine;

public class NetServer
{
    
    protected int port = 7777;
    protected int buffer = 4096;
    protected int tick = 100;
    protected int maxclient = 100;
    public NetServer()
    {
       
    }

    public void LoadConfig(object cfg) {
        Dictionary<string, object> dic = Parser.ObjectToDictionary(cfg);
        port = (int)dic["port"]; 
        maxclient = (int)dic["maxclient"]; 
        buffer = (int)dic["buffer"];
        tick = (int)dic["tick"];
    }
    

    public virtual void Start() { }
    public virtual void Poll() { }
    public virtual void Send(int connectionId, object data) { }
}
