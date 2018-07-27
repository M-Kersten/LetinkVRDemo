using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Oculus;
using UnityEngine.VR;

//When detecting movement, disable pausescreen and load intro level
public class DetectVRMovement : MonoBehaviour
{
    public Transform detectTransform;

    private Vector3 lastAxisRot;
    public float rotateDetectionDegree = 4.0f;

    [SerializeField]
    private float currentTimer;
    public float slowTimer = 3f;
    public float fastTimer = 0.5f;

    [SerializeField]
    private int idleCounts;
    public int idleThresshold = 5;

    private bool detectionEnabled = false;

    public string loadSceneOnMove;
    public string loadSceneOnRest;

    public GameObject loadMessage;
    
    private bool moving;

    [SerializeField]
    private bool disableDetection = false;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(3f);
        if (detectTransform)
        {
            lastAxisRot = detectTransform.eulerAngles;
            currentTimer = slowTimer;
            detectionEnabled = true;
            if (loadSceneOnMove != "")
            {
                StartCoroutine(DetectionLoop());
            }
        }

    }

    private IEnumerator DetectionLoop()
    {
        yield return new WaitForSeconds(currentTimer);
        if (OVRPlugin.userPresent)
        {
            idleCounts = 0;
            if (!moving)
            {
                Moved();
            }
        }
        /*
            if (Mathf.Abs(lastAxisRot.y - detectTransform.eulerAngles.y) > rotateDetectionDegree)
            {
                idleCounts = 0;
                if (!moving)
                {
                    Moved();
                }
            }*/
        
        else
        {
            if (moving)
            {
                idleCounts++;
                if (idleCounts >= idleThresshold)
                {
                    Resting();
                }
            }

            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.S))
            {
                if (loadMessage)
                {
                    loadMessage.SetActive(false);
                }
                moving = true;
                LevelManager.instance.SelectLevel("intro");
            }
        }
        //lastAxisRot = detectTransform.eulerAngles;
        if (detectionEnabled)
            StartCoroutine(DetectionLoop());
    }

    private void Moved()
    {
        idleCounts = 0;
        currentTimer = slowTimer;
        if (loadSceneOnMove != "")
        {
            if (loadMessage)
            {
                loadMessage.SetActive(false);
            }
            moving = true;
            LevelManager.instance.SelectLevel("intro");
            Debug.Log("moved so intro loaded");
        }

    }

    private void Resting()
    {
        moving = false;
        Debug.Log("to restmode");
        if (disableDetection)
            return;
        if (loadMessage.activeInHierarchy == false)
        {
            loadMessage.SetActive(true);
        }
        LevelManager.instance.SelectLevel("pause");
        idleCounts = 0;
        currentTimer = fastTimer;
    }
}
