using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIContainer 
{
    #region lazy singleton
    private static readonly Lazy<UIContainer> lazy =
        new Lazy<UIContainer>(() => new UIContainer());
    public static UIContainer Instance { get { return lazy.Value; } }
    GameObject uiroot;
    Dictionary<string, UIView> views;
    Stack<UIView> stack;
    #endregion
    UIContainer()
    {
        uiroot = GameObject.Find("UICanvas");
        views = new Dictionary<string, UIView>();
        stack = new Stack<UIView>();
    }
     T LoadUIView<T>() where T : UIView
    {
        return (T)LoadUIView(typeof(T).Name);
    }
    UIView LoadUIView(string UIName) {
        if (views.ContainsKey(UIName)) {
            return views[UIName];
        }
        else {
            GameObject UItmp = CreateGameObject(UIName);
            CanvasGroup cg = UItmp.GetComponent<CanvasGroup>() ? UItmp.GetComponent<CanvasGroup>() : UItmp.AddComponent<CanvasGroup>();
            cg.blocksRaycasts = false;
            UItmp.SetActive(false);
            UIView view = UItmp.GetComponent<UIView>();
            view.Init();
            
            views.Add(UIName, view);
            return view;
        }
       
    }
    GameObject CreateGameObject(string gameObjectName, GameObject parent = null)
    {
        GameObject prefab = Resources.Load<GameObject>("ui/"+gameObjectName);
        GameObject instanceTmp = GameObject.Instantiate(prefab);
        instanceTmp.name = prefab.name;

        instanceTmp.transform.SetParent(parent != null ? parent.transform : uiroot.transform);
        instanceTmp.transform.localPosition = Vector3.zero;
        instanceTmp.transform.localScale = Vector3.one;
        return instanceTmp;

    }

    //关闭当前
    public void OpenView(string UIName) {
        while (stack.Count>0)  PopView();
        PushView(UIName);
    }
    //不关闭当前，打开另外
    public void PushView(string UIName)
    {
        if (stack.Count > 0)
        {
            UIView v = stack.Peek();
            v.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
        UIView view = GetUIView(UIName);
        stack.Push(view);
        OnShow(view);
    }
     void PopView() {
        UIView view = stack.Pop();
        OnShow(view,false);
    }
    public void CloseView() {
        PopView();
        if (stack.Count > 0)
        {
            UIView view = stack.Peek();
            OnShow(view);
        }
    }

    void OnShow(UIView view,bool flag=true) {
        view.gameObject.SetActive(flag);
        view.GetComponent<CanvasGroup>().blocksRaycasts = flag;
        if (flag)
        {
            view.transform.SetAsLastSibling();
        }
    }
    public UIView GetUIView(string UIName) {
       
        return views.ContainsKey(UIName)? views[UIName]: LoadUIView(UIName);
        
    }
    public T GetUIView<T>() where T : UIView {
        string UIName = typeof(T).Name;
        if (views.ContainsKey(UIName)) {
            return (T)views[UIName];
        }
            return LoadUIView<T>();
    }
}
