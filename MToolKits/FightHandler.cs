using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Handler(2)]
public class FightHandler : MsgHandler
{
    public FightHandler(UnityApp app, int type) : base(app, type)
    {
       
    }
    public override void init()
    {
        RegisterMessage(3);
    }
    public override void OnLocalMessage(int msgCode, object message)
    {
        Debug.Log(msgCode+"->"+message);
        switch (msgCode)
        {
            
            default:
                break;
        }
    }

    public override void OnNetMessage(SocketMSG msg)
    {
       
    }
}
