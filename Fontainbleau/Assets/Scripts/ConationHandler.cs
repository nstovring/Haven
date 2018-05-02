using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConationHandler : MonoBehaviour {
    public bool isActive = false;
    [Range(0,6)]
    public int conationLevels;

    public float negativetimer = 0;
    public float positivetimer = 0;

    public float waitTimer = 5;
    // Use this for initialization
    public Transform caveBlocker;
    public Transform car;
    public GameStateSwitcher carStateSwitcher;

    public Transform forestBoids;
    public Transform orientalBoids;
    void Start () {

    }

    public void ActivateOrientalBoids()
    {
        orientalBoids.gameObject.SetActive(true);
    }

    public void ActivateForestBoids()
    {
        forestBoids.gameObject.SetActive(true);
    }


    public void DeActivateOrientalBoids()
    {
        orientalBoids.gameObject.SetActive(false);
    }

    public void DeActivateForestBoids()
    {
        forestBoids.gameObject.SetActive(false);
    }

    public void IncreaseConation()
    {
        caveBlocker.gameObject.SetActive(false);
        car.gameObject.SetActive(true);
        carStateSwitcher.isActive = true;
    }
    public KeyCode ActivateHelperKeyCode;
    public KeyCode DeActivateHelperKeyCode;

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyUp(ActivateHelperKeyCode))
        {
            ActivateForestBoids();
            GameStateHandler.Instance.ActivateHelper();
            negativetimer = 0;
        }
        if (Input.GetKeyUp(DeActivateHelperKeyCode))
        {
            //DeActivate helpers
            DeActivateForestBoids();
            GameStateHandler.Instance.DeActivateHelper();
            positivetimer = 0;
        }
        if (!isActive)
            return;

        if (conationLevels <= 3)
        {
            positivetimer = 0;
            negativetimer += Time.deltaTime;
        }
        else {
            negativetimer = 0;
            positivetimer += Time.deltaTime;
        }

        if(negativetimer > waitTimer)
        {
            //Activate helpers
            ActivateForestBoids();
            GameStateHandler.Instance.ActivateHelper();
            negativetimer = 0;
        }

        if (positivetimer > waitTimer)
        {
            //DeActivate helpers
            DeActivateForestBoids();
            GameStateHandler.Instance.DeActivateHelper();
            positivetimer = 0;
        }
    }
}
