using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateSwitcher : MonoBehaviour {

    public GameStateHandler.GameState GoToState;

    public List<Transform> deactivateTransforms;
    public List<Transform> activateTransforms;

    // Use this for initialization
    public bool isActive = true;

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void ActivateObjects()
    {
        foreach (var item in activateTransforms)
        {
            if (item.GetComponent<GameStateSwitcher>() != null)
                item.GetComponent<GameStateSwitcher>().isActive = true;
            item.gameObject.SetActive(true);
        }
    }

    void DeActivateObjects()
    {
        foreach (var item in deactivateTransforms)
        {
            if (item.GetComponent<GameStateSwitcher>() != null)
                item.GetComponent<GameStateSwitcher>().isActive = false;
            item.gameObject.SetActive(false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if(activateTransforms.Count > 0)
        {
            for (int i = 0; i < activateTransforms.Count; i++)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(activateTransforms[i].position, transform.position);
            }
           
        }

        if(deactivateTransforms.Count > 0)
        {
            for (int i = 0; i < deactivateTransforms.Count; i++)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(deactivateTransforms[i].position, transform.position);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive)
            return;
        if (other.transform.tag == "Player")
        {
            StartCoroutine(GameStateHandler.Instance.GoToState(GoToState));
            ActivateObjects();
            DeActivateObjects();
            transform.GetComponent<Collider>().enabled = false;
            isActive = false;
        }
    }
}
