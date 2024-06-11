using RiptideNetworking;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class NetManager 
{
    ClientNet net;
    Dictionary<int, List<Action<object>>> events;//服务器消息处理方法集
    public NetManager(Game game)
    {

        INIParser parser = new INIParser();
        parser.Open(Application.streamingAssetsPath + "/game.cfg");
        string net_class = parser.ReadValue("game", "net", "TelepathyNet");
        parser.Close();

        events = new Dictionary<int, List<Action<object>>>();
        net = Parser.CreateClass<ClientNet>(net_class);
        net.SetGame(game);
        Connect();
    }

    public void Connect() {
        net.Connect();
    }

    public void SendData(object data) {
        net.Send(data);
    }
    public void OnData(int id, Action<object> callback)
    {
        List<Action<object>> list = null;
        if (!events.ContainsKey(id))
        {
            list = new List<Action<object>>();
            list.Add(callback);
            events.Add(id, list);
            return;
        }
        list = events[id];
        list.Add(callback);
    }
    public void DispacthData(object data) {
        Dictionary<string, object> dic = (Dictionary<string, object>)data;
        int id = (int)dic["id"];
        if (!events.ContainsKey(id))
        {
            Debug.LogError("没有方法处理事件:" + id);
            return;
        }
        List<Action<object>> list = events[id];
        foreach (var a in list) a(data);
    }

    public void DispacthMessage(ClientMessageReceivedEventArgs e) {
        if (!events.ContainsKey(e.MessageId))
        {
            Debug.LogError("没有方法处理事件:" + e.MessageId);
            return;
        }
        List<Action<object>> list = events[e.MessageId];
        foreach (var a in list) a(e.Message);
    }

    internal void Poll()
    {
        net.Poll();
    }
}
