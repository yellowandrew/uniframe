using System;
using System.Collections.Generic;
using UnityEngine;

namespace BitMax {
public class EventSys {

   static Dictionary<int, List<Action<object>>> events = new Dictionary<int, List<Action<object>>>();
    public static void ListenEvent(int evCode, Action<object> callback)
    {
        List<Action<object>> list = null;
        if (!events.ContainsKey(evCode))
        {
            list = new List<Action<object>>();
            list.Add(callback);
            events.Add(evCode, list);
            return;
        }
        list = events[evCode];
        list.Add(callback);
    }
    public static void NotifyEvent(int evCode, object data) {
        if (!events.ContainsKey(evCode))
        {
            Debug.LogError("没有定义事件:" + evCode);
            return;
        }
        List<Action<object>> list = events[evCode];
        foreach (var a in list) a(data);
    }
    public static void RemoveEvent(int evCode, Action<object> callback) {
            if (!events.ContainsKey(evCode))
            {
                Debug.LogError("没有定义事件:" + evCode);
                return;
            }
            List<Action<object>> list = events[evCode];
            list.Remove(callback);
          //  if (list.Count==0)
           //     events.Remove(evCode);

        }

}

}
