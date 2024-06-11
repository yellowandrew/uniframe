using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Def;
using SPA;

public class UIView : MonoBehaviour
{

    public int type;
    void Start()
    {
        //OnInit();
    }

    public void init() {
        this.AutoLoad();
    }
    protected virtual void OnInit() {
      
    }

    public virtual void Open() {
        gameObject.SetActive(true);
    }
    public virtual void Close() {
        gameObject.SetActive(false);
    }

}
