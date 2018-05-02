using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFlasher : MonoBehaviour {

    public Image screenFlashImage;
    public float flashSpeed = 0.1f;
	// Use this for initialization
	void Start () {
        screenFlashImage = GetComponent<Image>();
        screenFlashImage.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
        StartCoroutine(StartFlash(Color.black));
    }

    public IEnumerator StartFlash(Color color)
    {
        Color flashColor = color;
        flashColor.a = 0;
        while (screenFlashImage.color.a < 1)
        {
            color.a += flashSpeed * 10 * Time.deltaTime;
            screenFlashImage.color = color;
            yield return new WaitForEndOfFrame();
        }

        flashColor = color;
        screenFlashImage.color = flashColor;
        while (screenFlashImage.color.a > 0)
        {
            flashColor.a -= flashSpeed * Time.deltaTime;
            screenFlashImage.color = flashColor;
            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator FlashTo()
    {
        Color flashColor = new Color(1, 1, 1, 0);
        while (screenFlashImage.color.a < 1)
        {
            flashColor.a += flashSpeed * 10 * Time.deltaTime;
            screenFlashImage.color = flashColor;
            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator FlashFrom()
    {
        Color flashColor = new Color(1, 1, 1, 1);
        screenFlashImage.color = flashColor;
        while (screenFlashImage.color.a > 0)
        {
            flashColor.a -= flashSpeed * Time.deltaTime;
            screenFlashImage.color = flashColor;
            yield return new WaitForEndOfFrame();
        }
    }



    IEnumerator Flash()
    {
        Color flashColor = Color.white;
        screenFlashImage.color = flashColor;
        while(screenFlashImage.color.a > 0)
        {
            flashColor.a -= flashSpeed * Time.deltaTime;
            screenFlashImage.color = flashColor;
            yield return new WaitForEndOfFrame();
        }
    }
	
	
}
