using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class AnimateMaterial : BasicPlayableBehaviour
{
	public ExposedReference<GameObject> ObjectToMove;
    public Color startValue;
    public Color endValue;
    public string parameterName;

    private GameObject _gameObject;
    private Color lerpValue;
    private Color originalValue;

	public override void OnGraphStart(Playable playable) 
	{
		_gameObject = ObjectToMove.Resolve(playable.GetGraph().GetResolver());
    }
		
	public override void ProcessFrame(Playable playable, FrameData info, object playerData) 
	{
        if (playable.GetTime() <= 0)
            return;
        _gameObject.GetComponent<MeshRenderer>().sharedMaterial.SetColor(parameterName, Color.Lerp(startValue, endValue, (float)(playable.GetTime() / playable.GetDuration())));
    }
}