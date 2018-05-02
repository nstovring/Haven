using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialoguePlayer : MonoBehaviour {
    public List<AudioClip> audioClip;
    private float secondDelay;
    public List<string> dialogueText;
    public bool isActive = true;
    public void Start()
    {
        if (audioClip == null)
        {
            isActive = false;
            return;
        }

        //secondDelay = audioClip.length;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive)
            return;
        if(other.transform.tag == "Player")
        {
            for (int i = 0; i < audioClip.Count; i++)
            {
                GameStateHandler.Instance.QueueAudioClip(audioClip[i], audioClip[i].length);
                GameStateHandler.Instance.QueueAudioSubtitle(dialogueText[i], audioClip[i].length);
            }
            GameStateHandler.Instance.PlayAudioSubtitleSequence();
            transform.GetComponent<Collider>().enabled = false;
            isActive = false;
        }
    }
}
