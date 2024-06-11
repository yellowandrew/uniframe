using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class AppLauncher : MonoBehaviour
{
  
    UnityApp app;

    void Awake()
    {
        app = new UnityApp();
    }
    void Start()
    {
        app.init(this);
        app.start();
    }

   
    void Update()
    {
        app.update(Time.deltaTime);
    }

    void FixedUpdate()
    {
        app.fixedUpdate(Time.deltaTime);
    }
    void OnApplicationQuit()
    {
        app.exit();
    }

    void OnApplicationFocus(bool focus)
    {
        app.focus(focus);
    }

    void OnApplicationPause(bool pause)
    {
        app.pause(pause);
    }
}
