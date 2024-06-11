using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIContainer 
{
    Transform UI_Root;
    Dictionary<int, UIView> dicts = new Dictionary<int, UIView>();
    public UIContainer()
    {
        UI_Root = GameObject.Find(Def.UI_ROOT).transform;
        loadUIPanels();
    }

    void loadUIPanels()
    {

        GameObject[] panelPrefabs = Resources.LoadAll<GameObject>(Def.UI_ViewDir);

        foreach (GameObject prefab in panelPrefabs)
        {
            GameObject go = GameObject.Instantiate(prefab);
            go.name = prefab.name;
            go.transform.SetParent(UI_Root, false);

            UIView view = go.GetComponent<UIView>();
            view.init();
            dicts.Add(view.type, view);
            go.SetActive(false);
        }
    }

    public UIView GetUI(int type)
    {
        if (dicts.ContainsKey(type))
        {
            return dicts[type];
        }
        return null;
    }

}
