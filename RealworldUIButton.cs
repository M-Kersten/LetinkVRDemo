using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRTK;
using UnityEngine.EventSystems;

//Make a button interactible with oculus controllers
public class RealworldUIButton : MonoBehaviour
{
    [Header("Make a button interactible with oculus controllers")]
    [Tooltip("Specify which button this is supposed to be")]
    public ButtonSelected selected;
    public RealWorldInteractible manager;
    public GameObject controller;
    [Tooltip("Length of vibration in seconds")]
    public float vibration;

    [HideInInspector]
    public Button button;
    private Color col;
    private Image img;

    void Start()
    {
        img = transform.GetChild(0).GetComponent<Image>();
        button = GetComponent<Button>();
        col = button.image.color;
        img.alphaHitTestMinimumThreshold = 0.2f;
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "controller")
        {
            VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(controller.gameObject), 1, vibration, vibration);
            manager.PlayButtonSound();
            button.Select();
            manager.selected = selected;
        }
    }

    public void Click()
    {
        button.onClick.Invoke();
    }
}
