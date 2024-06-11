using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ReflectMessage
{
    public sealed class MessageDispacther {
        #region lazy singleton
        private static readonly Lazy<MessageDispacther> lazy =
            new Lazy<MessageDispacther>(()=>new MessageDispacther());
        public static MessageDispacther Instance { get { return lazy.Value; } }
        #endregion
        MessageDispacther() {
            CreateMessagers();
        }

        Dictionary<ushort, Tuple<MethodInfo, object>> messagers = new Dictionary<ushort, Tuple<MethodInfo, object>>();
        void CreateMessagers() {
           
            MethodInfo[] methods = Assembly.GetCallingAssembly().GetTypes()
                                           .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance)) // Include instance methods in the search so we can show the developer an error instead of silently not adding instance methods to the dictionary
                                           .Where(m => m.GetCustomAttributes(typeof(OnMessageAttribute), false).Length > 0)
                                           .ToArray();

            List<Type> types = new List<Type>();//
            foreach (var method in methods) types.Add(method.ReflectedType);
            var messagerClasses = types.Distinct().ToArray();//

            foreach (var mc in messagerClasses) {
                object caller;
                if (mc.IsSubclassOf(typeof(MonoBehaviour))){
                    caller = GameObject.FindObjectOfType(mc);
                }else {
                    caller = Activator.CreateInstance(mc);
                }

                MethodInfo[] callerMethods = caller.GetType()
                                                     .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance)
                                                     .Where(m => m.GetCustomAttributes(typeof(OnMessageAttribute), false).Length > 0).ToArray();
                foreach (var method in callerMethods) {
                    OnMessageAttribute attribute = method.GetCustomAttribute<OnMessageAttribute>();
                    if (attribute != null)  {
                        var _ = Tuple.Create( method, caller);
                        messagers.Add(attribute.MessageId,_);
                    }
                }
            }

        }

        public void DispacthMessage(ushort messageId, params object[] args) {
            if (messagers.ContainsKey(messageId)) {
                var (methd, caller) = messagers[messageId];
                methd.Invoke(caller, new object[] { args });
            }
        }

    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class OnMessageAttribute : Attribute
    {
        public ushort MessageId { get;}
        public OnMessageAttribute(ushort messageId)
        {
            MessageId = messageId;
        }
    }

}
