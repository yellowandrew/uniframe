using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader 
{

    float delay = 1f;
    List<Scene> scenes;
    LoadingView loading;
    public SceneLoader()
    {
        scenes = new List<Scene>();
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnLoaded;
        loading = UIContainer.Instance.GetUIView<LoadingView>();
    }

     ~SceneLoader() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnLoaded;
    }
    private void OnSceneUnLoaded(Scene sc)
    {
        Debug.Log(sc.name +" Scene UnLoaded!!");
        scenes.Remove(sc);
    }

    private void OnSceneLoaded(Scene sc, LoadSceneMode mod)
    {
        Debug.Log(sc.name + " Scene Loaded!!");
        scenes.Add(sc);
    }
    bool HasScene(string sc) {
        foreach (Scene s in scenes)
        {
            if (s.name == sc)
                return true;
            
        }
        return false;
    }
    public IEnumerator LoadSceneAsync(string sceneName,Action callback=null) {
        if (!HasScene(sceneName))
        {
            //show loadingui
            UIContainer.Instance.OpenView(ViewType.LoadingView.ToString());
            float start_time = Time.time;
            AsyncOperation async = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            async.allowSceneActivation = false;

            while (!async.isDone)
            {
                
                if (Time.time - start_time >= delay)
                {
                    if (async.progress >= 0.9f && !async.allowSceneActivation)
                        async.allowSceneActivation = true;

                }
                loading.progress.value = async.progress;
                yield return null;
            }
            //hide loadingui
            UIContainer.Instance.CloseView();
           callback?.Invoke();

        }
        else { Debug.Log(sceneName + " allready Load!!"); }
       
    }

    public IEnumerator UnLoadSceneAsync(string sceneName, Action callback = null)
    {
        yield return SceneManager.UnloadSceneAsync(sceneName);
        callback?.Invoke();
    }
}
