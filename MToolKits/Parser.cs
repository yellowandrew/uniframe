using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
public class Parser
{
    public static byte[] ToBytes(object obj)
    {
        if (obj == null)
            return null;
        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();
        bf.Serialize(ms, obj);

        return ms.ToArray();
    }

    public static ArraySegment<byte> ToSegment(object obj)
    {
        byte[] bs = ToBytes(obj);
        return new ArraySegment<byte>(bs);
    }
    public static object ToObject(byte[] arrBytes)
    {
        MemoryStream memStream = new MemoryStream();
        BinaryFormatter binForm = new BinaryFormatter();
        memStream.Write(arrBytes, 0, arrBytes.Length);
        memStream.Seek(0, SeekOrigin.Begin);
        object obj = binForm.Deserialize(memStream);

        return obj;
    }

    public static byte[] Serialize(object data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream rems = new MemoryStream();
        formatter.Serialize(rems, data);
        return rems.GetBuffer();
    }

    public static object Deserialize(byte[] data)
    {
        System.Object obj = null;
        try
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream rems = new MemoryStream(data);
            data = null;
            obj = formatter.Deserialize(rems);
        }
        catch (Exception ex) { Debug.Log(ex.Message); }
        return obj;
    }


    public static Dictionary<string, object> ObjectToDictionary(object obj) {
        Dictionary<string, object> dictionary = new Dictionary<string, object>();

        Type type = obj.GetType();

        foreach (PropertyInfo propertyInfo in type.GetProperties())
        {
            string name = propertyInfo.Name;
            object value = propertyInfo.GetValue(obj, null);

            dictionary.Add(name, value);
        }

        return dictionary;
    }
	public static T DicToObject<T>(Dictionary<string, object> dic) where T : new() {
        var c = new T();

        try
        {
            foreach (var d in dic)
              c.GetType().GetProperty(d.Key).SetValue(c, d.Value);
           
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
       
        return c;
    }

}

