using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SPA;
using System;
using UnityEngine.UI;

[Serializable]
public class Cat {
    public void say(string w) {
        Debug.Log("喵喵喵!!!"+w);
    }
}
public class TestAttributeScript : MonoBehaviour
{
    [NewGameObject("ball")]
    public GameObject ball;

   [FindTag("Player")]
    public GameObject[] ply;
    [ComponentChild]
    public BoxCollider[] bc;
    [FindObject]
    public Light[]  lits;

    
    [Auto]
    public Cat cat;

    [Button("Canvas/Button", "hello", "汪汪汪")]
    public Button btn;

    [FindChildComponent("Canvas/Image")]
    public Image img;
    void Start()
    {
        this.AutoLoad();
    }

    public void hello(string w) {

        cat.say(w);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
