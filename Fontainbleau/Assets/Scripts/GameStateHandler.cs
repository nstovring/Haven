using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.Utility;

public class GameStateHandler : MonoBehaviour {

    public Transform Player;
    public Transform FrogPlayer;
    public Camera PlayerCam;

    public ConationHandler ConationHandler;
    public InteractionController InteractionController;
    public NotificationManager NotificationManager;
    public LightLoader lightLoader;
    public DollyCartManager dollyManager;
    public SceneLoader sceneLoader;
    public AudioManager AudioManager;
    public ScreenFlasher screenFlasher;
    public static GameStateHandler Instance;

    public enum GameState { Forest1, Cave1, Courtyard, Frog, Courtyard2, Cave2, Forest2, Cave3, ShishaRoom};
    public GameState gameState = GameState.Forest1;
    public bool intialize = false;

    internal IEnumerator GoToState(GameState goToState)
    {
        gameState = goToState;
        //load appropriate scene
        if (intialize)
        yield return StartCoroutine(sceneLoader.LoadScenesForState(goToState));

        //if (!initialized)
        //{
        //    yield return StartCoroutine(lightLoader.LoadLightScene(LightLoader.LightSettings.Day));
        //    yield return StartCoroutine(lightLoader.ChangeLightScene(LightLoader.LightSettings.Sunset));
        //    yield return StartCoroutine(lightLoader.ChangeLightScene(LightLoader.LightSettings.Night));
        //    initialized = true;
        //}

        switch (goToState)
        {
            case GameState.Forest1:
                //Daylight
                yield return StartCoroutine(lightLoader.LoadLightScene(LightLoader.LightSettings.Sunset));
                dollyManager.GetForestTrack();
                //Load forest scene & transition scene
                break;
            case GameState.Cave1:
                //Switch to sunset
                //Load courtyard & Shisharoom
                break;
            case GameState.Courtyard:
                //Load triggers for frogification
                break;
            case GameState.Frog:
                yield return StartCoroutine(GoToFrogMode());
                //Load wisps showing completion path & unload frog triggers
                break;
            case GameState.Courtyard2:
                yield return StartCoroutine(screenFlasher.FlashTo());
                yield return StartCoroutine(lightLoader.ChangeLightScene(LightLoader.LightSettings.Night));
                yield return StartCoroutine(GoToPlayerMode());
                break;
            case GameState.Cave2:
                //Switch to night light
                break;
            case GameState.Forest2:
                //activate conation handler
                //Loop until conation falls
                break;
            case GameState.Cave3:
                break;
            case GameState.ShishaRoom:
                //play end monologue
                yield return StartCoroutine(screenFlasher.FlashTo(Color.black));
                //End game
                Application.Quit();
                break;
        }
    }

    public GameState GetGameState()
    {
        return gameState;
    }

    public AudioClip introClip;
    public String introClipText;
  
	// Use this for initialization
	IEnumerator Start () {
        Instance = this;
        Player = GameObject.FindGameObjectWithTag("Player").transform;

        AudioManager.m_Owner = AudioManager;
        //load all scenes
        //yield return sceneLoader.LoadAndUnloadAllScenes();
        QueueAudioClip(introClip, introClip.length);
        QueueAudioSubtitle(introClipText, introClip.length);
        PlayAudioSubtitleSequence();
        yield return StartCoroutine(GoToState(gameState));
    }

    public void ActivateHelper()
    {
        dollyManager.boidFlock.nearbyDis = 0.1f;
        dollyManager.GetNearestCart();
    }

    public void DeActivateHelper()
    {
        dollyManager.boidFlock.nearbyDis = 5;
        dollyManager.boidTarget.transform.parent = null;
        dollyManager.boidTarget.transform.position += new Vector3(0, 100, 0);
    }

    private Vector3 playerCamOriginalLocalPos;
    public IEnumerator GoToFrogMode()
    {
        yield return StartCoroutine(screenFlasher.FlashTo());
        FrogPlayer.gameObject.SetActive(true);
        Player.transform.GetComponent<FirstPersonController>().enabled = false;
        playerCamOriginalLocalPos = PlayerCam.transform.localPosition;
        PlayerCam.transform.parent = null;
        PlayerCam.transform.GetComponent<SmoothFollow>().enabled = true;
        yield return StartCoroutine(screenFlasher.FlashFrom());

        float frogSectionTimer = 0;
        while (true)
        {
            frogSectionTimer += Time.deltaTime;
            yield return new WaitForEndOfFrame();

            if(frogSectionTimer > 4 * 60)
            {
                GoToPlayerMode();
            }
        }

    }

    public void PlayAudioSubtitleSequence()
    {
        AudioManager.StartLoop();
        NotificationManager.StartLoop();
    }

    public void QueueAudioClip(AudioClip clip, float delay)
    {
        AudioManager.actions.Enqueue(AudioManager.playDialogue(clip, delay));
    }

    public void QueueAudioSubtitle(string text, float delay)
    {
        NotificationManager.actions.Enqueue(NotificationManager.ShowSubtitle(text, delay));
    }

    public IEnumerator GoToPlayerMode()
    {
        
        PlayerCam.transform.GetComponent<SmoothFollow>().enabled = false;
        PlayerCam.transform.parent = Player.transform;
        while(Vector3.Distance(PlayerCam.transform.localPosition, playerCamOriginalLocalPos) > 0.1f)
        {
            PlayerCam.transform.localPosition = Vector3.Lerp(PlayerCam.transform.localPosition, playerCamOriginalLocalPos, 0.1f);
            yield return new WaitForEndOfFrame();
        }
        Player.transform.GetComponent<FirstPersonController>().enabled = true;
        FrogPlayer.gameObject.SetActive(false);
        yield return StartCoroutine(screenFlasher.FlashFrom());
    }

    // Update is called once per frame
    void Update () {
        //if (Input.GetKeyUp(KeyCode.F))
        //{
        //    StartCoroutine(GoToFrogMode());
        //}
        //
        //if (Input.GetKeyUp(KeyCode.V))
        //{
        //    StartCoroutine(GoToPlayerMode());
        //}
    }
}
