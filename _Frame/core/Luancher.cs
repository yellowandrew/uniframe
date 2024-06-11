using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Luancher : MonoBehaviour
{
    Game game;
    private void Awake()
    {
        //DontDestroyOnLoad(gameObject);
        Application.runInBackground = true;
        Application.targetFrameRate = 30;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        INIParser ini = new INIParser();
        ini.Open(Application.streamingAssetsPath + "/game.cfg");
        string gameclass = ini.ReadValue("game", "class", "Game");
        ini.Close();
        game = Assembly.GetExecutingAssembly().CreateInstance(gameclass) as Game;

    }
    void Start()
    {
        game.Start(this);
    }

   
    void Update()
    {
        game.Update(Time.deltaTime);
    }

    void LateUpdate()
    {
        game.LateUpdate();
    }

    private void FixedUpdate()
    {
        game.FixedUpdate();
    }

    void OnApplicationQuit()
    {
        game.Quit();
    }
}
