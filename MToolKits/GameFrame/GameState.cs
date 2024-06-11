using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameState
{
    protected UnityApp app;
    public void SetApp(UnityApp app) {
        this.app = app;
    }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void OnEnter() { }
    public virtual void OnExit() { }
    public virtual void OnLoadLevel() { }
    public virtual void Dispose() { }
    public void Enter()
    {
        OnEnter();
    }

    public void Exit()
    {
        OnExit();
    }

    public virtual void OnPauseGame()
    {

    }

    public virtual void OnQuitGame()
    {

    }

    public virtual void OnResume()
    {

    }

    public virtual void OnBackKey() { }
}
