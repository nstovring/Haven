using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class InteractableObject : MonoBehaviour {

    public bool m_selected;
    public bool m_lookedAt;
    private PostProcessVolume m_volume;
    public Transform postProcessPrefab;
    private Transform m_postProcessPrefab;

    public string description = "";

    public bool stateChanger = false;
    public bool isFrogTransforms = false;
    public GameStateSwitcher gameStateSwitcher;
    public DialoguePlayer dplayer;
    // Use this for initialization
    void Start () {
        transform.tag = "InteractableObject";
        m_postProcessPrefab = Instantiate(postProcessPrefab, transform);
        m_postProcessPrefab.localPosition = Vector3.zero;
        m_volume = m_postProcessPrefab.GetComponent<PostProcessVolume>();
    }

    public void ShowMessage()
    {
        NotificationManager.Instance.SetDescription(description);
        NotificationManager.Instance.ShowDescription();
    }

    public void RemoveMessage()
    {
        NotificationManager.Instance.RemoveDescription();
    }

    // Update is called once per frame
    void Update () {
        if (m_selected)
        {
            m_volume.weight = 1;
        }
        else
        {
            m_volume.weight = 0;
        }
	}
}
