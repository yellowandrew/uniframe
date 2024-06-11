using kcp2k;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerLuancher : MonoBehaviour
{
   
    ServerProxy server;
    private void Awake()
    {
        Application.runInBackground = true;
        Application.targetFrameRate = 30;
        QualitySettings.vSyncCount = 0;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Log.Info = Debug.Log;
        Log.Warning = Debug.LogWarning;
        Log.Error = Debug.LogError;

    }
   
    void Start()
    {
        server = new ServerProxy();
        server.Start();
    }

    void Update()
    {
        
    }

    private void LateUpdate()
    {
       
    }
    private void FixedUpdate()
    {
        server.Poll();
    }
    private void OnApplicationQuit()
    {
       
    }
}
