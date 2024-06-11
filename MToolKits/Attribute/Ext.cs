using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using UnityEngine;

public static class Ext 
{
    public static void AutoSet(this MonoBehaviour mono) {
        var fields = mono.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        foreach (var field in fields){
            AutoAttribute attr = field.GetCustomAttribute(typeof(AutoAttribute), true) as AutoAttribute;
            if (attr != null){
                attr.Set(mono, field);
            }
        }
    }
}

[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
public class AutoAttribute : Attribute
{
    public string[] nameList;
    public AutoAttribute(params string[] list){
        nameList = list;
    }

    public void Set(MonoBehaviour mono, FieldInfo field)
    {
        string typeName = field.FieldType.FullName.Replace("[]", string.Empty);
        Type type = field.FieldType.Assembly.GetType(typeName);
        var name = field.Name;
        if (field.FieldType.IsArray)
        {
            IList listToBeSet=null;
            if (nameList.Length == 1){
                name = nameList[0];
                var root = mono.transform.Find(name);
                int count = root.childCount;
                listToBeSet = Array.CreateInstance(type, count);
                for (int i = 0; i < count; i++){
                    listToBeSet[i] = root.GetChild(i).GetComponent(type);
                }
            }
            else if(nameList.Length > 1){
                listToBeSet = Array.CreateInstance(type, nameList.Length);
                for (int i = 0; i < nameList.Length; i++){
                    var t = mono.transform.Find(nameList[i]);
                    if (t !=null)
                    {
                        listToBeSet[i] = t.GetComponent(type);
                    }
                    
                }
            }

            field.SetValue(mono, listToBeSet);
        }
        else
        {
            if (nameList.Length == 1) name = nameList[0];
            
            if (type == typeof(GameObject)){
                var t = mono.transform.Find(name);
                if (t != null)
                {
                    var found = mono.transform.Find(name).gameObject;
                    field.SetValue(mono, found);
                }
               
            }
            else{
                var t = mono.transform.Find(name);
                if (t != null)
                {
                    var found = mono.transform.Find(name).GetComponent(field.FieldType);
                    field.SetValue(mono, found);
                }
                
            }
        }
   
    }
}
