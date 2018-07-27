using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

//Change material of selected 3D object and child objects
public class Colorpicker : MonoBehaviour {
    [Header("Change material of selected 3D object and child objects")]
    [Tooltip("Starting material")]
    public Material mat;
    public VRTK_ControllerEvents R_controller;
    public VRTK_Pointer pointer;
    private Transform currentTarget;

    void Start()
    {
        pointer.DestinationMarkerSet += ChangeColor;
    }

    public void ChangeColor(object sender, DestinationMarkerEventArgs e)
    {
        if (e.target.tag == "colorObj")
        {
            currentTarget = e.target;
            Debug.Log("Changing material of: " + e.target.name);
            ChangeNow();
        }
    }

    public void ChangeNow()
    {
        if (currentTarget.GetComponent<Renderer>())
        {
            currentTarget.GetComponent<Renderer>().material = mat;
        }

        for (int i = 0; i < currentTarget.transform.childCount; i++)
        {
            if (currentTarget.GetChild(i).GetComponent<Renderer>())
            {
                currentTarget.transform.GetChild(i).GetComponent<Renderer>().material = mat;
            }
        }
    }

    public void SetMaterial(Material m)
    {
        mat = m;
    }
}
