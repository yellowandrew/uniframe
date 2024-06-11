
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using UnityEngine;

public  class UnityApp
{
    MonoBehaviour _mono;
    protected NetManager netManager;
    Dictionary<int, MsgHandler> handlers;

    protected UIContainer uiContainer;
    public MonoBehaviour Mono {
        get { return _mono; }
    }
    public GameState CurGameState { get; private set; }
    GameState lastGameState;
    protected Dictionary<int, GameState> states;
    //存要切换的状态,然后下一帧切换
    int changeStateNextFrame = -1;
    public virtual void init(MonoBehaviour mono) {
        _mono = mono;
        Application.runInBackground = true;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        netManager = new NetManager();
        netManager.OnNetMSG = OnNetMessage;

        uiContainer = new UIContainer();

        handlers = new Dictionary<int, MsgHandler>();
        SetupHandlers();

        states = new Dictionary<int, GameState>();
        SetupStates();
        if (states.Count == 0)
        {
            Debug.LogError("需要一个游戏GameState");
        }
        else {
            ChangeState(0);
        }
        
  }

    protected void SetupHandlers() {
        Assembly asm = Assembly.GetExecutingAssembly();
        Type[] types = asm.GetExportedTypes();
        foreach (var t in types)
        {
            if (t.IsClass)
            {
                HandlerAttribute attr = t.GetCustomAttribute(typeof(HandlerAttribute), false) as HandlerAttribute;
                if (attr != null)
                {
                    MsgHandler h = Activator.CreateInstance(t, new object[] {this, attr.type }) as MsgHandler;
                    handlers.Add(attr.type, h);
                    h.init();
                }
            }
        }
    }

    void SetupStates() {
        Assembly asm = Assembly.GetExecutingAssembly();
        Type[] types = asm.GetExportedTypes();
        foreach (var t in types)
        {
            if (t.IsClass)
            {
                GameStateAttribute attr = t.GetCustomAttribute(typeof(GameStateAttribute), false) as GameStateAttribute;
                if (attr != null)
                {
                    GameState s = Activator.CreateInstance(t, new object[] { }) as GameState;
                    s.SetApp(this);
                    states.Add(attr.type, s);
                }
            }
        }
    }
    public UIView GetView(int type) {
        return uiContainer.GetUI(type);
    }
    public virtual void start() { }
    public virtual void update(float delta)
    {
        netManager.update();
        if (lastGameState != null)
        {
            lastGameState.Exit();
            lastGameState = null;
            Resources.UnloadUnusedAssets();
            GC.Collect(2);
        }
 
        if (CurGameState != null)
        {
            CurGameState.Update();
        }

        OnUpdate();

    }
    protected virtual void OnUpdate()
    {
        if (changeStateNextFrame > -1)
        {
            if (CurGameState != null)
            {
                //Save the state
                lastGameState = CurGameState;
            }
            CurGameState = CreateState(changeStateNextFrame);
            CurGameState.Enter();
            changeStateNextFrame = -1;
        }

    }
    public void ChangeState(int changeToState)
    {
        changeStateNextFrame = changeToState;
    }
    protected virtual GameState CreateState(int statEnum)
    {
        if (states.ContainsKey(statEnum))
        {
            return states[statEnum];
        }
        return null;

    }
    protected void AddState(int statEnum, GameState state)
    {
        states.Add(statEnum, state);
    }

    protected void RemoveState(int statEnum)
    {
        if (states.ContainsKey(statEnum))
        {
            GameState gs = states[statEnum];
            gs.Dispose();
            states.Remove(statEnum);
        }
    }
    #region 消息分发处理
    Dictionary<int, List<MsgHandler>> dict = new Dictionary<int, List<MsgHandler>>();
    public void BindMessage(int type, int msgCode) {
        List<MsgHandler> list = null;
        //之前没有注册过
        if (!dict.ContainsKey(msgCode))
        {
            list = new List<MsgHandler>();
            list.Add(handlers[type]);
            dict.Add(msgCode, list);
            return;
        }

        //之前注册过
        list = dict[msgCode];
        list.Add(handlers[type]);
    }
    void OnNetMessage(SocketMSG msg) {
        handlers[msg.OpCode].OnNetMessage(msg);
    }
    public void RemoveMessage(int msgCode, MsgHandler hd)
    {
        //没注册过 没法移除 报个警告
        if (!dict.ContainsKey(msgCode))
        {
            Debug.LogWarning("消息码没有注册 ：-> " + msgCode);
            return;
        }

        List<MsgHandler> list = dict[msgCode];

        if (list.Count == 1)
            dict.Remove(msgCode);
        else
            list.Remove(hd);
    }
    public void EmitMessage( int msgCode, object message) {
        if (!dict.ContainsKey(msgCode))
        {
            Debug.LogWarning("消息码没有注册 ：-> " + msgCode);
            return;
        }
        //一旦注册过这个消息 给所有的脚本 发过去
        List<MsgHandler> list = dict[msgCode];
        for (int i = 0; i < list.Count; i++)
        {
            list[i].OnLocalMessage(msgCode, message);
        }
    }


    public void ConnectServer() {
        netManager.ConnectServer();
    }
    public void SendNetMessage(SocketMSG MSG)
    {
        netManager.SendData(MSG);
    }
    public void SendNetMessage(int opCode, int subCode, int command, object message)
    {
        netManager.SendData( opCode,  subCode,  command,  message);
    }
    #endregion

    public virtual void pause(bool pause) { }
    public virtual void focus(bool focus) { }

    protected virtual void OnInit()
    {
        Debug.Log("游戏初始化");
    }
    protected virtual void load() { }
    protected virtual void save() { }

    public virtual void fixedUpdate(float delta)
    {
        if (CurGameState != null)
        {
            CurGameState.FixedUpdate();
        }
    }
    public virtual void exit() {
        netManager.exit();
    }
}
