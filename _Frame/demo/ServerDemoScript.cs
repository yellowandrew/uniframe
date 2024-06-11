using RiptideNetworking;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class ServerDemoScript : MonoBehaviour
{
    Image image1;
    Button button2;

   
    void Start()
    {
       // test1("abc");
      //  test1();
       
    }

    void test1(string m=null) {
        Debug.Log(m??"¹þ¹þ");
    }

    int test2(GlobalState gs) {
        return gs switch
        {
            GlobalState.LoadingState => 1,
            //GlobalState.MenuState => 2,
           // GlobalState.PlayState => 3,
            _ => 0
        };
       
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    [OnMessage(1)]
    void OnLoginReq(NetServer server, int connectionId, object data) {

        Message msg = data as Message;
        //Debug.Log($"{msg.GetString()},{msg.GetString()},{msg.GetString()}");
        byte[] bs = msg.GetBytes();
        LoginData login = Parser.ToObject<LoginData>(bs);
        Debug.Log($"{login.name},{login.password},{login.email}");
        // Dictionary<string, object> dic = (Dictionary<string, object>)data;
        // Debug.Log($"{dic["name"]},{dic["password"]},{dic["email"]}");
        // LoginData login = data as LoginData;
        // Debug.Log($"{login.name},{login.password},{login.email}");
        // string msg = data.ToString();
        // Debug.Log($"{dic["name"]},{dic["password"]},{dic["email"]}");
        // msg += " back to you!!";

        //  var obj = new { id = 2, name = "jacky", password = "kacky123", email = "jacker@abc.com" };
        // Dictionary<string, object> dic2 = Parser.ObjectToDictionary(obj);
        // server.Send(connectionId, dic2);

        Message m = Message.Create(MessageSendMode.unreliable, 2);
      
        login.name = "jacky";
        login.password = "kacky123";
        login.email = "joker@abc.com";
        m.AddBytes(Parser.ToBytes(login));
        // m.Add("jacky");
        // m.Add("kacky123");
        // m.Add("joker@abc.com");
        server.Send(connectionId, m);
    }

}

[System.Serializable]
public class LoginData {
    public string name;
    public string password;
    public string email;
}

