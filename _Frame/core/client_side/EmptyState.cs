using BitMax;
using RiptideNetworking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyState : State
{

    public EmptyState() { }

    public override void Set(Game game)
    {
        base.Set(game);
        game.Net.OnData(2, OnLoginRep);
        game.Messager.ListenMessage(44, (data) => {
            Debug.Log(data);
        });
    }
    public override void Update(float delta)
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            // game.LoadScene("A");
            //UIContainer.Instance.OpenView(ViewType.MenuView.ToString());
          
            var obj = new {id = 1, name = "tomy", password = "123abc", email = "tomy@123.com" };
            Dictionary<string, object> dic = Parser.ObjectToDictionary(obj);
           Game.Net.SendData(dic);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            // Game.Messager.NotifyMessage(44, "¹þ¹þ");
            // UIContainer.Instance.PushView(ViewType.ShopView.ToString());
            // game.UnLoadScene("A",()=> {
            //  game.LoadScene("B");
            //  });
            // LoadingView loadingView =  UIContainer.Instance.LoadUIView<LoadingView>();

           // var obj = new { id = 1, name = "tomy", password = "123abc", email = "tomy@123.com" };
           // Dictionary<string, object> dic = Parser.ObjectToDictionary(obj);
          //  byte[] bs = Parser.ToBytes(dic);

            Message m = Message.Create(MessageSendMode.unreliable, 1);

            LoginData login = new LoginData();
            login.name = "tomy";
            login.password = "asdqwe";
            login.email = "tomy@123.com";
            m.AddBytes(Parser.ToBytes(login));

          // m.Add("tomy");
         //  m.Add("123abc");
          // m.Add("tomy@123.com");
            Game.Net.SendData(m);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            UIContainer.Instance.CloseView();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            UIContainer.Instance.OpenView(ViewType.ShopView.ToString());
        }
    }

    void OnLoginRep(object data)
    {
        //Dictionary<string, object> dic = (Dictionary<string, object>)data;
        // Debug.Log($"{dic["name"]},{dic["password"]},{dic["email"]}");
      
        Message msg = data as Message;
        byte[] bs = msg.GetBytes();
        LoginData login = Parser.ToObject<LoginData>(bs);
        Debug.Log($"{login.name},{login.password},{login.email}");
        // Debug.Log($"{msg.GetString()},{msg.GetString()},{msg.GetString()}");
    }
}
