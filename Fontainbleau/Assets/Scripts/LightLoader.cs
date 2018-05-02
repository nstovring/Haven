using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LightLoader : MonoBehaviour {

    public enum LightSettings { Sunset, Day, Night};
    public LightSettings l_Settings = LightSettings.Day;

    private static int currentLightBuildIndex;
	// Use this for initialization
	void Start () {
    }

    void InitializeLights()
    {
    }

    public int GetLightBuildIndex(LightSettings l_settings)
    {
        switch (l_settings)
        {
            case LightSettings.Day:
                currentLightBuildIndex = 3;
                break;
            case LightSettings.Night:
                currentLightBuildIndex = 4;
                break;
            case LightSettings.Sunset:
                currentLightBuildIndex = 5;
                break;
        }
        return currentLightBuildIndex;
    }

    public IEnumerator LoadLightScene(LightSettings l_settings)
    {
        currentLightBuildIndex = GetLightBuildIndex(l_settings);
        yield return StartCoroutine(LoadScenes(currentLightBuildIndex));
    }

    public IEnumerator ChangeLightScene(LightSettings setting)
    {
        yield return StartCoroutine(UnLoadScenes(currentLightBuildIndex));
        currentLightBuildIndex = GetLightBuildIndex(setting);
        yield return StartCoroutine(LoadScenes(currentLightBuildIndex));
    }


    public IEnumerator LoadScenes(int scene)
    {

        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    public IEnumerator UnLoadScenes(int scene)
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.
       
        AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(scene);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    // Update is called once per frame
    void Update () {
        //if (Input.GetKeyUp(KeyCode.N))
        //{
        //    ChangeLightScene(LightSettings.Night);
        //}
        //if (Input.GetKeyUp(KeyCode.M))
        //{
        //    ChangeLightScene(LightSettings.Sunset);
        //}
    }
}
