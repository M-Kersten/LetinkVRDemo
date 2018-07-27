using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMessage : MonoBehaviour
{

    [SerializeField]
    private Transform trackTransform;
    public bool stayOn = false;
    public float rotateSpeed;
    public float height;
    public GameObject message;
    public GameObject[] messageShadow;
    public GameObject knop;
    public float messageDuration;
    public float fadeSpeed;

    private float rotY;
    private Material m_Material;
    private Material[] m_MaterialShadow = new Material[4];
    private Color m_Color;
    private bool textVisible = false;

    private void Awake()
    {        
        m_Material = message.GetComponent<Renderer>().material;
        for (int i = 0; i < messageShadow.Length; i++)
        {
            m_MaterialShadow[i] = messageShadow[i].GetComponent<Renderer>().material;
        }
        m_Color = m_Material.color;
    }

    void Start()
    {
        message.GetComponent<MeshRenderer>().sortingLayerName = "UIFront";
        transform.position = new Vector3(trackTransform.position.x, trackTransform.position.y + height, trackTransform.position.z);
        transform.rotation = new Quaternion(trackTransform.rotation.x, trackTransform.rotation.y, trackTransform.rotation.z, 0);
    }

    void Update()
    {
        transform.position = new Vector3(trackTransform.position.x, trackTransform.position.y + height, trackTransform.position.z);
        rotY = Mathf.LerpAngle(transform.localEulerAngles.y, trackTransform.localEulerAngles.y, Time.deltaTime * rotateSpeed);
        transform.localEulerAngles = new Vector3(0f, rotY, 0f);
    }

    IEnumerator AlphaFadeIn()
    {
        knop.SetActive(false);
        float alpha = 0.0f;
        while (alpha < 1.0f)
        {
            alpha += fadeSpeed * Time.deltaTime;
            m_Material.color = new Color(m_Color.r, m_Color.g, m_Color.b, alpha);
            for (int i = 0; i < messageShadow.Length; i++)
            {
                m_MaterialShadow[i].color = new Color(m_Color.r, m_Color.g, m_Color.b, alpha);
            }
            yield return null;
        }
        if (!stayOn)
        {
            Invoke("ToTransparent", messageDuration);
        }
        else
        {
            if (knop)
            {
                knop.SetActive(true);
            }
        }
    }

    private IEnumerator AlphaFadeOut()
    {
        knop.SetActive(false);
        //Debug.Log("fading out");
        float alpha = 1.0f;
        while (alpha > 0.0f)
        {
            alpha -= fadeSpeed * Time.deltaTime;
            m_Material.color = new Color(m_Color.r, m_Color.g, m_Color.b, alpha);
            for (int i = 0; i < messageShadow.Length; i++)
            {
                m_MaterialShadow[i].color = new Color(m_Color.r, m_Color.g, m_Color.b, alpha);
            }

            yield return null;
        }
    }

    public void EnableDisable()
    {
        if (textVisible)
        {
            StartCoroutine(AlphaFadeOut());
            textVisible = false;
        }
        else
        {
            StartCoroutine(AlphaFadeIn());
            textVisible = true;
        }
    }

    public void SetNewText(string setText)
    {
        CancelInvoke();
        string newText = setText.Replace("newline", "\n");
        message.GetComponent<TextMesh>().text = newText;
            for (int i = 0; i < messageShadow.Length; i++)
            {
                messageShadow[i].GetComponent<TextMesh>().text = newText;
            }
        if (setText == "")
        {
            stayOn = false;
        }
        ToOpaque();
        
    }

    public void SetNewText(string setText, bool autoFadeOut)
    {
        stayOn = !autoFadeOut;
        CancelInvoke();
        string newText = setText.Replace("newline", "\n");
        message.GetComponent<TextMesh>().text = newText;
            for (int i = 0; i < messageShadow.Length; i++)
            {
                messageShadow[i].GetComponent<TextMesh>().text = newText;
            }        
        ToOpaque();
    }

    public void ToOpaque()
    {
        StartCoroutine(AlphaFadeIn());
    }
    
    public void ToTransparent()
    {
        StartCoroutine(AlphaFadeOut());
    }

}
