using RiptideNetworking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class ServerDispacther 
{
    private static readonly Lazy<ServerDispacther> lazy =
         new Lazy<ServerDispacther>(() => new ServerDispacther());
    public static ServerDispacther Instance { get { return lazy.Value; } }
    ServerDispacther()
    {
        CreateMessagers();
    }

    Dictionary<int, Tuple<MethodInfo, object>> messagers = new Dictionary<int, Tuple<MethodInfo, object>>();
    void CreateMessagers()
    {

        MethodInfo[] methods = Assembly.GetCallingAssembly().GetTypes()
                                       .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance)) // Include instance methods in the search so we can show the developer an error instead of silently not adding instance methods to the dictionary
                                       .Where(m => m.GetCustomAttributes(typeof(OnMessageAttribute), false).Length > 0)
                                       .ToArray();

        List<Type> types = new List<Type>();//
        foreach (var method in methods) types.Add(method.ReflectedType);
        var messagerClasses = types.Distinct().ToArray();//

        foreach (var mc in messagerClasses)
        {
            object caller;
            if (mc.IsSubclassOf(typeof(MonoBehaviour)))
            {
                caller = GameObject.FindObjectOfType(mc);
            }
            else
            {
                caller = Activator.CreateInstance(mc);
            }

            MethodInfo[] callerMethods = caller.GetType()
                                                 .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance)
                                                 .Where(m => m.GetCustomAttributes(typeof(OnMessageAttribute), false).Length > 0).ToArray();
            foreach (var method in callerMethods)
            {
                OnMessageAttribute attribute = method.GetCustomAttribute<OnMessageAttribute>();
                if (attribute != null)
                {
                    var _ = Tuple.Create(method, caller);
                    messagers.Add(attribute.MessageId, _);
                }
            }
        }

    }

    public void DispacthData(NetServer server, int connectionId, object data) {
        Dictionary<string, object> dic = (Dictionary<string, object>)data;
        int id = (int)dic["id"];
        if (messagers.ContainsKey(id))
        {
            var (methd, caller) = messagers[id];
            methd.Invoke(caller, new object[] { server, connectionId, data });
        }
    }
    public void DispacthMessage(NetServer server, ServerMessageReceivedEventArgs e) {
        if (messagers.ContainsKey(e.MessageId))
        {
            var (methd, caller) = messagers[e.MessageId];
            methd.Invoke(caller, new object[] { server, e.FromClientId, e.Message });
        }
    }

}
[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public class OnMessageAttribute : Attribute
{
    public ushort MessageId { get; private set; }
    public OnMessageAttribute(ushort messageId)
    {
        MessageId = messageId;
    }
}