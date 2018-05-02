using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {

    const int PlayerScene = 9;
    const int Forest = 10;
    const int Transition = 11;
    const int CourtandShisha = 12;
    public List<int> toLoadSceneNames;
    public List<int> toUnLoadsceneNames;

    int[] sceneBuildIndexes = { 9, 10, 11 };
    public IEnumerator LoadScenesForState(GameStateHandler.GameState gameState)
    {
        toLoadSceneNames = new List<int>();
        toUnLoadsceneNames = new List<int>();

        switch (gameState)
        {
            case GameStateHandler.GameState.Forest1:
                //toLoadSceneNames.Add(PlayerScene);
                toLoadSceneNames.Add(Forest);
                toLoadSceneNames.Add(Transition);
                toLoadSceneNames.Add(CourtandShisha);
                break;
            case GameStateHandler.GameState.Cave1:
                //toUnLoadsceneNames.Add(Forest);
                //toLoadSceneNames.Add(CourtandShisha);
                break;
            case GameStateHandler.GameState.Courtyard:
                break;
            case GameStateHandler.GameState.Frog:
                break;
            case GameStateHandler.GameState.Courtyard2:
                //toLoadSceneNames.Add(Forest);
                break;
            case GameStateHandler.GameState.Cave2:
                //toUnLoadsceneNames.Add(CourtandShisha);
                break;
            case GameStateHandler.GameState.Forest2:
                break;
            case GameStateHandler.GameState.Cave3:
                //toLoadSceneNames.Add(CourtandShisha);
                break;
            case GameStateHandler.GameState.ShishaRoom:
                break;
        }

        if (toLoadSceneNames.Count > 0)
            yield return StartCoroutine(LoadScenes());
        if (toUnLoadsceneNames.Count > 0)
            yield return StartCoroutine(UnLoadScenes());
    }

    public IEnumerator LoadAndUnloadAllScenes()
    {
        for (int i = 10; i < 13; i++)
        {
            toLoadSceneNames.Add(i);
            toUnLoadsceneNames.Add(i);
        }

        if (toLoadSceneNames.Count > 0)
            yield return StartCoroutine(LoadScenes());
        if (toLoadSceneNames.Count > 0)
            yield return StartCoroutine(UnLoadScenes());

    }

    public IEnumerator UnLoadScenes()
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.
        if (toUnLoadsceneNames.Count < 1)
            yield return 0;

        AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(toUnLoadsceneNames[0]);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        for (int i = 1; i < toLoadSceneNames.Count; i++)
        {
            asyncLoad = SceneManager.UnloadSceneAsync(toUnLoadsceneNames[i]);

            // Wait until the asynchronous scene fully loads
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

        }
    }

    public IEnumerator LoadScenes()
    {
    
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(toLoadSceneNames[0], LoadSceneMode.Additive);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        for (int i = 1; i < toLoadSceneNames.Count; i++)
        {
            asyncLoad = SceneManager.LoadSceneAsync(toLoadSceneNames[i], LoadSceneMode.Additive);

            // Wait until the asynchronous scene fully loads
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

        }
    }
}
