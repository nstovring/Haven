using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class NotificationManager : MonoBehaviour {

    public static NotificationManager Instance;
    Canvas m_canvas;
    public TextMeshProUGUI notificationText;
    public TextMeshProUGUI descriptionText;

    public Queue<IEnumerator> actions = new Queue<IEnumerator>();
    Coroutine m_InternalCoroutine = null;

    bool descriptionActive = false;

    public Vector3 textPosition;
    public Vector3 outOfScreenPosition;

    public void SetDescription(string d)
    {
        descriptionText.text = d;
    }

    public IEnumerator ShowSubtitle(string text, float wait)
    {
        descriptionActive = true;
        descriptionText.text = text;
        descriptionText.transform.gameObject.SetActive(true);
        yield return new WaitForSeconds(wait);
        descriptionActive = false; 
        descriptionText.transform.gameObject.SetActive(false);
        Debug.Log("Remove Text");
    }

    public void ShowDescription()
    {
        descriptionActive = true;

        descriptionText.transform.gameObject.SetActive(true);
    }

    public void RemoveDescription()
    {
        descriptionActive = false;
        descriptionText.transform.gameObject.SetActive(false);
    }

    public void StartLoop()
    {
        m_InternalCoroutine = m_Owner.StartCoroutine(Process());
    }

    MonoBehaviour m_Owner = null;
    private IEnumerator Process()
    {
        while (true)
        {
            if (actions.Count > 0)
                yield return m_Owner.StartCoroutine(actions.Dequeue());
            else
                yield return null;
        }
    }

    // Use this for initialization
    void Start () {
        Instance = this;
        m_Owner = this;
    }
	
	// Update is called once per frame
	void Update () {
	}
}
