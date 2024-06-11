using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MsgHandler
{
    public int type;
    protected UnityApp app;
    public MsgHandler(UnityApp app, int type)
    {
        this.app = app;
        this.type = type;
    }

    public virtual void init() { }

    protected void Emit(int msgCode, object message)
    {
        app.EmitMessage(msgCode, message);
    }
    protected void RegisterMessage(int msgCode)
    {
        app.BindMessage(type, msgCode);
    }
    protected void UnRegisterMessage(int msgCode)
    {
        app.RemoveMessage(msgCode, this);
    }
    public abstract void OnLocalMessage(int msgCode, object message);

    public abstract void OnNetMessage(SocketMSG msg);
    protected void SendNetMessage(SocketMSG MSG)
    {
        app.SendNetMessage(MSG);
    }
    protected void SendNetMessage(int opCode, int subCode, int command, object message)
    {
        app.SendNetMessage(opCode, subCode, command, message);
    }
}
