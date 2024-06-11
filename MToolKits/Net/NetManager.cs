using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetManager 
{

    NetSocket netSocket;
    public Action<SocketMSG> OnNetMSG;
    public NetManager() {
        netSocket = new NetSocket();
    }
    public void ConnectServer() {
        netSocket.Connect(Def.SERVER_IP, Def.SERVER_PORT);
    }

    public void SendData(SocketMSG MSG) {
        netSocket.Send(MSG);
    }
    public void SendData(int opCode, int subCode, int command, object message)
    {
        netSocket.Send( opCode,  subCode,  command,  message);
    }
    public void update()
    {
        if (netSocket == null)
            return;

        while (netSocket.SocketMsgQueue.Count > 0)
        {
            SocketMSG msg = netSocket.SocketMsgQueue.Dequeue();
            //处理消息
            processSocketMsg(msg);
        }
    }
    private void processSocketMsg(SocketMSG msg)
    {
        OnNetMSG?.Invoke(msg);
    }

    public void exit() {
        netSocket.Close();
    }
}
