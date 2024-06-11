using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Messager 
{
    Dictionary<int, List<Action<object>>> messages;
   // public delegate void MessageDelegate<T>(T data);
   
    public Messager()
    {
        messages = new Dictionary<int, List<Action<object>>>();
    }
    public void ListenMessage(int id, Action<object> callback) {
        List<Action<object>> list;
        if (!messages.ContainsKey(id))
        {
            list = new List<Action<object>>();
            list.Add(callback);
            messages.Add(id, list);
            return;
        }
        list = messages[id];
        list.Add(callback);
    }
    public void NotifyMessage(int id, object data) {
        if (!messages.ContainsKey(id))
        {
            Debug.LogError("没有定义事件:" + id);
            return;
        }
        List<Action<object>> list = messages[id];
        foreach (var a in list) a(data);
    }
    public void RemoveMessage(int id, Action<object> callback) {
        if (!messages.ContainsKey(id))
        {
            Debug.LogError("没有定义事件:" + id);
            return;
        }
        List<Action<object>> list = messages[id];
        list.Remove(callback);
        //  if (list.Count==0)
        //     events.Remove(id);
    }
}
