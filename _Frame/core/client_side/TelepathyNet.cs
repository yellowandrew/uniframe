using System;
using System.Collections;
using System.Collections.Generic;
using Telepathy;
using UnityEngine;

public class TelepathyNet : ClientNet
{
    Client client;
    public TelepathyNet()
    {
        client = new Client(size);
        client.OnConnected += OnConnected;
        client.OnDisconnected += OnDisconnected;
        client.OnData += OnData;
    }

    public override void Connect()
    {
        client.Connect(ip, port);
    }
    public override void Poll()
    {
        client.Tick(tick);
    }

    public override void Send(object data)
    {
        byte[] bs = Parser.ToBytes(data);
        client.Send(new ArraySegment<byte>(bs));
    }

    private void OnConnected()
    {
        Debug.Log(" Connected");
    }

    private void OnDisconnected()
    {
        Debug.Log(" Disconnected");
    }

    private void OnData(ArraySegment<byte> data)
    {
        game.Net.DispacthData(Parser.ToObject(data.Array));
    }

   
}
