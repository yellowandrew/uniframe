using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    protected Game Game { get { return _game; } }
    Game _game;
    public virtual void Set(Game game)
    {
        _game = game;
    }
    public virtual void Enter() { }
    public virtual void Update(float delta) { }
    public virtual void Exit() { }
    public virtual void Resume() { }
    public virtual void Pause() { }
}

public class StateStack {
    Stack<int> stack;
    Dictionary<int, State> states;
    State currentState;
    public StateStack()
    {
        stack = new Stack<int>();
        states = new Dictionary<int, State>();
    }
    public void Update(float delta)
    {
        currentState?.Update(delta);
    }
    public void AddState(int id, State state) {
        if (states.ContainsKey(id)) return;
        states.Add(id, state);
    }

    public void PushState(int id) {
        SwitchState(id);
    }
    public void ChangeState(int id) {
        SwitchState(id, true);
    }
    void SwitchState(int id,bool replace = false) {
        if (!states.ContainsKey(id))
        {
            Debug.LogError("state:" + id + " is null!!");
            return;
        }
        if (currentState != null && Top() == id)
        {
            Debug.LogError("state:" + id + " is on top now!");
            return;
        }
        if (Top() > 0) {
            if (replace)   {
                states[Top()].Exit();
                stack.Pop();
            }  else  {
                states[Top()].Pause();
            }
        }

        currentState = states[id];
        currentState.Enter();
        stack.Push(id);
    }

    public void PopState()
    {
        if (Top() < 0) return;
        states[Top()].Exit();

        stack.Pop();
        if (Top() > 0)
        {
            states[Top()].Resume();
            currentState = states[Top()];
        }
    }

    bool IsOnTop(int id) {
        return (currentState != null && Top() == id) ;
    }

    int Top()
    {
        return stack.Count > 0 ? stack.Peek() : -1;
    }
    public void Clean() {
        states.Clear();
        stack.Clear();
        currentState = null;
    }
}
