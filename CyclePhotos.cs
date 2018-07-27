using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Location
{
    public string title;
    public Texture2D photo;
}

public class CyclePhotos : MonoBehaviour {

    public Location[] photos;
    public float photoDuration;
    [SerializeField]
    private float timeLeft;
    private int i = 0;
    public UIMessage message;
    public float fadeTime;
    private Color faderColor;
    [SerializeField]
    private float alpha;
    private Material m;
    private bool fadingIn;
    private bool fadingOut;

    void Start () {
        m = GetComponent<MeshRenderer>().material;
        //m.SetColor("_Color", faderColor);
        m.SetTexture("_MainTex", photos[i].photo);
        
        timeLeft = photoDuration;
	}

    void Update()
    {
        if (isActiveAndEnabled)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft < 0)
            {
                NextPhoto();
                timeLeft = photoDuration;
            }
        }
        else
        {
            timeLeft = photoDuration;
        }
        if (fadingIn)
        {
            if (alpha < 1)
            {
                alpha += Time.deltaTime * fadeTime;
                m.SetColor("_Color", new Color(1, 1, 1, alpha));
            }
            else
            {
                fadingIn = false;
                alpha = 1;
            }
        }
        if (fadingOut)
        {
            if (alpha > 0)
            {
                alpha -= Time.deltaTime * fadeTime;
                m.SetColor("_Color", new Color(1, 1, 1, alpha));
            }
            else
            {
                fadingOut = false;
                alpha = 0;
            }
        }

    }
    
    public void NextPhoto()
    {
        i++;
        if (i > photos.Length - 1)
        {
            i = 0;
        }
        StartCoroutine(StartFader());
        
    }

    public IEnumerator StartFader()
    {
        fadingOut = true;
        yield return new WaitForSeconds(fadeTime + (fadeTime/2));
        m.SetTexture("_MainTex", photos[i].photo);

        if (photos[i].title != "")
        {
            message.SetNewText(photos[i].title, true);
        }
        fadingIn = true;
    }
    
    public void FadeIn()
    {
        if (alpha < 1)
        {
            alpha += Time.time * fadeTime;
            m.SetColor("_Color", new Color(alpha, alpha, alpha));
        }
        alpha = 1;
    }

    public void FadeOut()
    {
        if (alpha > 0)
        {
            alpha -= Time.time * fadeTime;
            m.SetColor("_Color", new Color(alpha, alpha, alpha));
        }
        alpha = 0;
    }
}
