using LiteNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using UnityEngine;

public class NetConnection : LiteNetwork.TcpClient
{
    IParser parser;
    Dictionary<UInt32, Tuple<MethodInfo, Handler>> handleActions
        = new Dictionary<UInt32, Tuple<MethodInfo, Handler>>();
    public NetConnection(IParser parser, string address, int port) : base(address, port)
    {

        this.parser = parser;
        var handleClasses = GetType().Assembly.GetTypes().Where(x => x.IsSubclassOf(typeof(Handler)));
        foreach (var cs in handleClasses)
        {
            var handler = Activator.CreateInstance(cs) as Handler;

            foreach (var meth in handler.GetType().GetMethods())
            {
                var attrib = meth.GetCustomAttribute<PackageHandleAttribute>();
                if (attrib == null) continue;
                handleActions.Add(attrib.id, new Tuple<MethodInfo, Handler>(meth, handler));
            }
        }
    }
    public void DisconnectAndStop()
    {
        _stop = true;
        DisconnectAsync();
        while (IsConnected)
            Thread.Yield();
    }

    protected override void OnConnected()
    {
        Debug.Log($" TCP client connected a new session with Id {Id}");
        Debug.Log(handleActions.Count);
    }

    protected override void OnDisconnected()
    {
        Debug.Log($" TCP client disconnected a session with Id {Id}");

        // Wait for a while...
        Thread.Sleep(1000);

        // Try to connect again
        if (!_stop)
            ConnectAsync();
    }

    protected override void OnReceived(byte[] buffer, long offset, long size)
    {
        var package = parser.ReadPackageFromBuffer(buffer);
        if (handleActions.TryGetValue(package.id, out var item))
        {
            var (m, h) = item;
            m.Invoke(h, new object[] { this, package });
        }
    }

    public void SendPackage(Package package)
    {
        Send(parser.WritePackageToBuffer(package));
    }
    public void SendPackageAsyn(Package package)
    {
        SendAsync(parser.WritePackageToBuffer(package));
    }

    protected override void OnSent(long sent, long pending)
    {
        //Debug.Log($"OnSent  {sent} >{pending}");
    }

    protected override void OnEmpty()
    {
        //Debug.Log($"OnEmpty ");
    }

    protected override void OnError(SocketError error)
    {
        Debug.Log($"Chat TCP client caught an error with code {error}");
    }

    private bool _stop;
}
