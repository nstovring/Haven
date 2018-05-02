using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {

    public AudioSource dialogueAudioSource;
    public AudioSource musicAudioSource;
    public AudioMixerSnapshot ToDialogue;
    public AudioMixerSnapshot ToAmbience;

    public Queue<IEnumerator> actions = new Queue<IEnumerator>();
    Coroutine m_InternalCoroutine = null;

    public IEnumerator playDialogue(AudioClip clip, float audioLength) {
        Debug.Log("playing dialogue");
        dialogueAudioSource.PlayOneShot(clip);   
        ToDialogue.TransitionTo(0.5f);  
        yield return new WaitForSeconds(audioLength);
        Debug.Log("Stop Sound");
        ToAmbience.TransitionTo(0.5f);
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

    private void Start()
    {
        m_Owner = this;
    }

    void playerDialogue()
    {

    }


}
