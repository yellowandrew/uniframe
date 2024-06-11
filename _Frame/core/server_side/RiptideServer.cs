using RiptideNetworking;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiptideServer : NetServer
{
    Server server;

    public RiptideServer()
    {
        server = new Server();
        server.ClientConnected += OnConnected;
        server.ClientDisconnected += OnDisconnected;
        server.MessageReceived += OnData;
    }

    public override void Start()
    {
        server.Start((ushort)port, (ushort)maxclient);
    }
    public override void Poll()
    {
        server.Tick();
    }
    public override void Send(int connectionId, object data)
    {
        server.Send((Message)data, (ushort)connectionId);
    }
    private void OnConnected(object sender, ServerClientConnectedEventArgs e)
    {
        Debug.Log("OnClientConnected:" + e.Client.Id);
    }

    private void OnDisconnected(object sender, ClientDisconnectedEventArgs e)
    {
        Debug.Log("OnClientDisconnected:" + e.Id);
    }

    private void OnData(object sender, ServerMessageReceivedEventArgs e)
    {
       
        ServerDispacther.Instance.DispacthMessage(this, e);
    }
}
