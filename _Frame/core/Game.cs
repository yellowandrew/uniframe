using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BitMax;
using System.Reflection;

public class Game
{
    SceneLoader scene;
    protected MonoBehaviour mono;
    protected StateStack stateStack;

    public NetManager Net { get { return _net; } }
    protected NetManager _net;

    public Messager Messager { get { return _msger; } }
    protected Messager _msger;
    int state = 0;
    public Game()
    {
        
        scene = new SceneLoader();
        stateStack = new StateStack();
        _msger = new Messager();

        INIParser parser = new INIParser();
        parser.Open(Application.streamingAssetsPath + "/game.cfg");
        state = parser.ReadValue("game", "state", 0);
        parser.Close();
    }

    void LoadState() {
        foreach (var gs in Enum.GetValues(typeof(GlobalState)))
        {

            State state = Parser.CreateClass<State>(gs.ToString());
            state.Set(this);
            stateStack.AddState((int)gs, state);
        }
    }

    public void PopState()
    {
        stateStack.PopState();
    }
    public void PushState(int id)
    {
        stateStack.PushState(id);
    }
    public void ChangeState(int id)
    {
        stateStack.ChangeState(id);
    }

    public void LoadScene(string sc, Action callback = null) {
        mono.StartCoroutine(scene.LoadSceneAsync(sc,callback));
    }
    public void UnLoadScene(string sc, Action callback = null) {
        mono.StartCoroutine(scene.UnLoadSceneAsync(sc,callback));
    }
    public void Start(MonoBehaviour mono)
    {
        this.mono = mono;
        LoadState();
        ChangeState(state);
    }
    public void Update(float delta)
    {
        stateStack.Update(delta);
    }
    public virtual void LateUpdate() {
       
    }
    public virtual void FixedUpdate() { }
    public void Quit() { stateStack.Clean(); }

}

public  class NetGame : Game {
   
    public NetGame()
    {
        _net = new NetManager(this);
    }

    public override void FixedUpdate()
    {
        _net.Poll();
    }

}
