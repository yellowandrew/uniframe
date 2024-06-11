using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class SimpleEvent 
{
    
    private static Dictionary<int, UnityEvent> eventDictionary = new Dictionary<int, UnityEvent>();


    public static void BindEvent(int eventCode, UnityAction callBack) {
        if (eventDictionary.TryGetValue(eventCode, out UnityEvent thisEvent))
        {
            thisEvent.AddListener(callBack);
        }
        else
        {
            thisEvent = new UnityEvent();
            thisEvent.AddListener(callBack);
            eventDictionary.Add(eventCode, thisEvent);
       
        }

    }

    public static void UnBindEvent(int eventCode, UnityAction callBack)
    {
        if (eventDictionary.TryGetValue(eventCode, out UnityEvent thisEvent))
        {
            thisEvent.RemoveListener(callBack);
        }
    }

    public static void EmitEvent(int eventCode, float delay=0)
    {
      
        if (eventDictionary.TryGetValue(eventCode, out UnityEvent thisEvent))
        {
            if (delay <= 0)
            {
                thisEvent.Invoke();
            }
            else
            {
                int d = (int)(delay * 1000);
                DelayedInvoke(thisEvent, d);
            }
        }
    }

    private static async void DelayedInvoke(UnityEvent thisEvent, int delay)
    {
        await Task.Delay(delay);
        thisEvent.Invoke();
    }

    public static void Clear()
    {
        foreach (KeyValuePair<int, UnityEvent> evnt in eventDictionary)
        {
            evnt.Value.RemoveAllListeners();
        }
        eventDictionary = new Dictionary<int, UnityEvent>();
    }
}
