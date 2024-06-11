using System;
using System.Collections;
using System.Collections.Generic;
using Telepathy;
using UnityEngine;

public class TelepathyServer :NetServer
{
    Server server;
    public TelepathyServer()
    {
        server = new Server(buffer);
        server.OnConnected = OnConnected;
        server.OnDisconnected = OnDisconnected;
        server.OnData = OnData;
    }
    public override void Start()
    {
        server.Start(port);
    }

    public override void Poll()
    {
        server.Tick(tick);
    }
    private void OnConnected(int connectionId)
    {
        Debug.Log(connectionId + " Connected");
    }

    private void OnDisconnected(int connectionId)
    {
        Debug.Log(connectionId + " Disconnected");
    }

    private void OnData(int connectionId, ArraySegment<byte> data)
    {
        ServerDispacther.Instance.DispacthData(this, connectionId, Parser.ToObject(data.Array));
    }

    public override void Send(int connectionId, object data)
    {
        server.Send(connectionId, Parser.ToSegment(data));
    }
}
