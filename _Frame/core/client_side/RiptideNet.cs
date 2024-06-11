using RiptideNetworking;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiptideNet : ClientNet
{
    Client client;
    public RiptideNet()
    {
        client = new Client();
        client.Connected += OnConnected;
        client.ConnectionFailed += OnConnectionFailed;
        client.ClientDisconnected += OnClientDisconnected;
        client.Disconnected += OnDisconnected;
        client.MessageReceived += OnData;
    }

    public override void Send(object data)
    {
        
        client.Send((Message)data);
    }
    private void OnData(object sender, ClientMessageReceivedEventArgs e)
    {
        game.Net.DispacthMessage(e);
    }

    private void OnDisconnected(object sender, EventArgs e)
    {
        
    }

    private void OnClientDisconnected(object sender, ClientDisconnectedEventArgs e)
    {
      
    }

    private void OnConnectionFailed(object sender, EventArgs e)
    {
       
    }

    private void OnConnected(object sender, EventArgs e)
    {
       
    }

    public override void Connect()
    {
       
        client.Connect($"{ip}:{port}");
    }
    public override void Poll()
    {
        client.Tick();
    }
}
