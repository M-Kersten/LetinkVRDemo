using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus;
using VRTK;
using UnityEngine.UI;

public class ThreeDPainter : MonoBehaviour {

    public GameObject trailObject;
    public Transform trailHolder;
    public Transform controller;

    private TrailRenderer tr;
    private Color startColor;
    private Color endColor;
    private bool on;
    private GameObject instantiatedTrail;
    private Material mat;

    private void Start()
    {
        tr = trailObject.GetComponent<TrailRenderer>();
        startColor = tr.startColor;
        endColor = tr.endColor;
        mat = tr.sharedMaterial;
    }

    void Update () {
        if (!on)
        {
            if (OVRInput.Get(OVRInput.Button.One))
            {
                tr = trailObject.GetComponent<TrailRenderer>();
                tr.startColor = startColor;
                tr.endColor = endColor;
                tr.material = mat;
                instantiatedTrail = (GameObject)Instantiate(trailObject, controller.position, Quaternion.identity, controller);
                tr = instantiatedTrail.GetComponent<TrailRenderer>();
                on = true;
            }
        }
        else
        {
            if (!OVRInput.Get(OVRInput.Button.One))
            {
                instantiatedTrail.transform.parent = trailHolder;
                on = false;
            }
        }
    }

    public void ClearDrawing()
    {
        foreach (Transform child in trailHolder)
        {
            GameObject.Destroy(child.gameObject);
        }
        tr = trailObject.GetComponent<TrailRenderer>();
    }

    public void SetColor(GameObject obj)
    {
        startColor = obj.GetComponent<Image>().color;
        endColor = obj.GetComponent<Image>().color;
    }
   
    public void SetBrush(Material m)
    {
        mat = m;
    }
}
