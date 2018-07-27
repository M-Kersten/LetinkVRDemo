using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using Oculus;

public class PlaceObjects : MonoBehaviour {

    public GameObject[] furniture;
    //gameobject to store placed gameobjects
    public Transform itemHolder;
    //material used when moving the gameobject
    public Material highlightedMat;
    public VRTK_Pointer pointer;
    //object to place in front of pointer to release focus from groundplane (not optimal)
    public GameObject rayBreaker;
    
    private float scalingFactor;
    private DestinationMarkerEventArgs tempE;
    private object tempSender;
    private bool chosenNewObject;
    private Vector3 place;
    private GameObject currentObj;
    private Quaternion objRotation;
    private GameObject selected;
    private Material startMat;

    public static PlaceObjects instance;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        objRotation = Quaternion.Euler(-90, 0, 0);
        pointer.DestinationMarkerSet += PlaceObj;
        pointer.DestinationMarkerEnter += CreateObj;
        pointer.DestinationMarkerHover += CursorPos;
        currentObj = null;
    }

    //spawn object
    public void CreateObj(object sender, DestinationMarkerEventArgs e)
    {
        if (pointer.IsActivationButtonPressed())
        {
            //place new object
            if (e.target.tag == "vloer")
            {
                if (!selected)
                {
                    if (currentObj)
                    {
                        tempSender = sender;
                        tempE = e;
                        place.Set(e.destinationPosition.x, 0, e.destinationPosition.z);
                        selected = (GameObject)Instantiate(currentObj, place, objRotation);
                        if (selected.GetComponent<Renderer>())
                        {
                            startMat = selected.GetComponent<Renderer>().material;
                        }
                        selected.transform.localScale = new Vector3(.6f, .6f, .6f);
                        selected.transform.parent = itemHolder;
                    }
                }
            }
            if (e.target.tag == "placedObject")
            {
                Debug.Log("object interactible");
                if (!selected)
                {
                    selected = e.target.gameObject;
                    startMat = selected.GetComponent<Renderer>().material;
                    selected.tag = "Untagged";
                    selected.layer = 10;
                }
            }
        }
    }

    //place the object
    public void PlaceObj(object sender, DestinationMarkerEventArgs e)
    {
        if (selected)
        {
            selected.tag = "placedObject";
            selected.layer = 11;
        }
        
        ChangeMaterial(startMat, selected);
        selected = null;
    }

    //update position of the active object
    public void CursorPos(object sender, DestinationMarkerEventArgs e)
    {
        if (chosenNewObject)
        {
            rayBreaker.SetActive(true);
            Invoke("ResetLayer", .1f);
        }
        else
        {
            if (selected)
            {
                ChangeMaterial(highlightedMat, selected);
                place.Set(e.destinationPosition.x, 0, e.destinationPosition.z);
                selected.transform.position = place;
                selected.transform.Rotate(0, 0, Input.GetAxis("HorizontalJoy"));

                //check if object is too big
                if (selected.transform.localScale.x > 1f)
                {
                    if (Input.GetAxis("VerticalJoy") > 0)
                    {
                        scalingFactor = 0f;
                    }
                    else
                    {
                        //Debug.Log("scaling freely");
                        scalingFactor = Input.GetAxis("VerticalJoy") * .012f;
                    }
                }
                else
                {
                    scalingFactor = Input.GetAxis("VerticalJoy") * .012f;
                }

                //check if object is too small
                if (selected.transform.localScale.x < .2f)
                {
                    if (Input.GetAxis("VerticalJoy") < 0)
                    {
                        scalingFactor = 0f;
                    }
                    else
                    {
                        scalingFactor = Input.GetAxis("VerticalJoy") * .012f;
                    }
                }
                else if (selected.transform.localScale.x < 1f)
                {
                    scalingFactor = Input.GetAxis("VerticalJoy") * .012f;
                }

                //apply scaling
                selected.transform.localScale += new Vector3(scalingFactor, scalingFactor, scalingFactor);
            }
        }
    }

    public void RemoveObj(object sender, DestinationMarkerEventArgs e)
    {
        Destroy(selected);
    }

    void ChangeMaterial(Material newMat, GameObject selectedObj)
    {
        if (selected)
        {
            Renderer[] children;
            children = selectedObj.GetComponentsInChildren<Renderer>();
            foreach (Renderer rend in children)
            {
                var mats = new Material[rend.materials.Length];
                for (var j = 0; j < rend.materials.Length; j++)
                {
                    mats[j] = newMat;
                }
                rend.materials = mats;
            }
        }
    }
    public void SetObject(int i)
    {
        currentObj = furniture[i];
        chosenNewObject = true;
        Debug.Log("object " + i + " chosen");
    }

    private void ResetLayer()
    {
        rayBreaker.SetActive(false);
        chosenNewObject = false;
    }

    public void DeleteAll()
    {
        foreach (Transform child in itemHolder.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

}
