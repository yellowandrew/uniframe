using GameProtocol;
using Protocol.dto;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Handler(1)]
public class LoginHandler : MsgHandler
{

    LoginView loginView;
    public LoginHandler(UnityApp app,int type) : base(app, type) {
        
    }

    public override void init()
    {
        base.init();
        app.ConnectServer();
        loginView = app.GetView(0) as LoginView;

        loginView.LoginBtn.onClick.AddListener(() =>
        {
            AccountDto accountDto = new AccountDto();
            accountDto.account = "Andzo";
            accountDto.password = "1234qwer";
            SendNetMessage(GameProtocol.Protocol.login, 0, LoginProtocol.Login_C_Req, "1234qwer");
        });

        loginView.RegisterButton.onClick.AddListener(() =>
        {
            Debug.Log("注册");
            AccountDto accountDto = new AccountDto();
            accountDto.account = "Andzo";
            accountDto.password = "1234qwer";
            SendNetMessage(GameProtocol.Protocol.login, 0, LoginProtocol.Reg_C_Req, accountDto);
        });
        // 
        RegisterMessage(8);
        
    }
    public override void OnLocalMessage(int msgCode, object message)
    {
        switch (msgCode)
        {
            case 8:
                loginView.Open();
                break;
            default:
                break;
        }
    }

    public override void OnNetMessage(SocketMSG msg)
    {
        //Debug.Log("登录模块接收到服务器消息");

        //Emit(msg.Command, "登录成功进入战斗！");
        switch (msg.SubCode)
        {
            case LoginProtocol.Login_S_Rep:
                processLogin(msg.Command);
                break;
            case LoginProtocol.Reg_S_Rep:
                processRegister(msg.Command);
                break;
            default:
                break;
        }
    }


    void processLogin(int code) {
        switch (code)
        {
            case 0:
                Debug.Log("登录成功");
                break;
            case -1:
                Debug.Log("账号未注册");
                break;
            case -2:
                Debug.Log("已经登录");
                break;
            case -3:
                Debug.Log("密码或用户错误");
                break;
            case -4:
                Debug.Log("密码或用户为空");
                break;
            default:
                break;
        }
    }
    void processRegister(int code) {
        switch (code)
        {
            case 0:
                Debug.Log("注册成功");
                break;
            case 1:
                Debug.Log("账号已被占用");
                break;
            default:
                break;
        }


    }
}
