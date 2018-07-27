using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using UnityEngine.VR;
using System;
using UnityEngine.UI;

public enum LevelLighting
{
    day,
    foggy,
    night,
    foggynight,
    panorama,
    distantFog
}

public enum HandMenu
{
    none,
    interieur,
    colorpicker,
    brush
}
[System.Serializable]
public class Level
{
    public string name;
    public GameObject level;
    public string levelIntroText;
    [Tooltip("Sets lighting conditions in level")]
    public LevelLighting levelLighting;
    [Tooltip("Activates panel on your left hand")]
    public HandMenu menu;
    public bool joystickMovement;
    [Tooltip("Movement speed of joystick, if movement disabled leave at 0")]
    public float moveSpeed;
    [Tooltip("Enables arc/bezier teleporter")]
    public bool teleportEnabled;
    [Tooltip("Enables the whole groundplane to be teleportable")]
    public bool teleportableGround;
    [Tooltip("Disables menus and panels and enables visible oculus controllers")]
    public bool tutorialLevel;
    public bool panoramaLevel;
    [Tooltip("Enables stereo cameras on oculus OVRManager")]
    public bool stereoPanorama;
    public bool groundDisabled;
    public bool seperateLightSource;
}
//Central manager of levels
public class LevelManager : MonoBehaviour
{
    [Header("Manage your levels by adding levels to the levels array")]
    [Tooltip("If enabled, the game will start at the pausecreen and enable the intro level when the game starts")]
    public bool startAtStart;

    [Header("Manager")]
    public Level[] scenes;
    [SerializeField]
    private Level currentLevel;
    [SerializeField]
    private List<Level> previouslevels = new List<Level>();
    private bool firstLevel = true;

    [Space(5)]
    [Header("Oculus")]
    public GameObject player;
    public GameObject OVRCamera;
    public GameObject playerAvatar;

    private Vector3 playerStartPos;
    private Vector3 playerAvatarStartPos;

    [Header("VRTK")]
    public VRTK_ControllerEvents L_controller;
    public VRTK_ControllerEvents R_controller;
    public VRTK_SlideObjectControlAction slideX;
    public VRTK_SlideObjectControlAction slideY;

    private VRTK_BezierPointerRenderer L_pointer;
    private VRTK_BezierPointerRenderer R_pointer;

    [Header("Ingame objects")]
    public GameObject ground;
    [Tooltip("Nadir is a circle that hides the bottom part of a 360 photo or video")]
    public GameObject nadir;
    public GameObject VRMenuMain;
    public GameObject[] VRMenuPanels;
    public GameObject trailRenderer;
    public UIMessage uIMessage;
    public UIMenu levelSelectMenu;
    //these are required to reset these objects back to starting positions when leaving the level
    public RespawnIfFallen LDrumStick;
    public RespawnIfFallen RDrumStick;
    public RespawnIfFallen toverstaf;
    
    private OvrAvatar avatar;
    private AudioSource sound;

    [Header("Visual")]
    public Material skyboxMat;
    public Material skyboxDarkMat;
    public Material groundWhite;
    public Material groundDark;
    public Color fogLight;
    public Color fogDark;
    public Light sun;
    
    public static LevelManager instance;

    void Awake()
    {
        instance = this;
        avatar = playerAvatar.GetComponent<OvrAvatar>();
        sound = GetComponent<AudioSource>();
    }

    void Start()
    {
        L_pointer = L_controller.GetComponent<VRTK_BezierPointerRenderer>();
        playerStartPos = player.transform.position;
        playerAvatarStartPos = playerAvatar.transform.position;
        if (startAtStart)
        {
            //Should be made more flexible
            SelectLevel("pause");
        }
    }

    public void SelectLevel(string level)
    {
        //unload current level
        if (!firstLevel)
        {
            previouslevels.Add(currentLevel);
            SetScenesInactive();
            ResetPos();
            ResetToStartMode();
        }
        else
        {
            firstLevel = false;
        }
        uIMessage.ToTransparent();

        //Load new level
        scenes[FindElementByName(level, scenes)].level.SetActive(true);
        currentLevel = scenes[FindElementByName(level, scenes)];
        Debug.Log("Next level: " + currentLevel.name);
        if (currentLevel.levelIntroText != "")
        {
            if (FindElementByName(currentLevel.name, previouslevels) == -1)
            {
                uIMessage.SetNewText(currentLevel.levelIntroText, false);
            }
            else
            {
                uIMessage.SetNewText(currentLevel.levelIntroText, true);
            }
        }
        //Set conditions of new level according to level class
            switch (currentLevel.levelLighting)
            {
                case LevelLighting.day:
                    SetToLight();
                    FogOnOff(false);
                    break;
                case LevelLighting.foggy:
                    SetToLight();
                    FogOnOff(true);
                    break;
                case LevelLighting.night:
                    SetToDark();
                    FogOnOff(false);
                    break;
                case LevelLighting.foggynight:
                    SetToDark();
                    FogOnOff(true);
                    break;
                case LevelLighting.distantFog:
                    SetFogTo();
                    break;
                case LevelLighting.panorama:
                    FogOnOff(false);
                    break;
                default:
                    break;
            }
            switch (currentLevel.menu)
            {
                case HandMenu.none:
                    SwitchToPanel("");
                    trailRenderer.SetActive(false);
                    break;
                case HandMenu.interieur:
                    SwitchToPanel("interieurCanvas");
                    trailRenderer.SetActive(false);
                    break;
                case HandMenu.colorpicker:
                    SwitchToPanel("kleurCanvas");
                    trailRenderer.SetActive(false);
                    break;
                case HandMenu.brush:
                    SwitchToPanel("tekenCanvas");
                    trailRenderer.SetActive(true);
                    break;
                default:
                    break;
            }
            if (currentLevel.joystickMovement)
            {
                slideX.maximumSpeed = currentLevel.moveSpeed;
                slideY.maximumSpeed = currentLevel.moveSpeed;
            }
            else
            {
                slideX.maximumSpeed = 0f;
                slideY.maximumSpeed = 0f;
            }

            VRMenuMain.SetActive(!currentLevel.tutorialLevel);
            avatar.ShowControllers(currentLevel.tutorialLevel);

            if (currentLevel.panoramaLevel)
            {
                currentLevel.joystickMovement = false;
                currentLevel.teleportEnabled = false;
                currentLevel.groundDisabled = true;
            }
            ground.SetActive(!currentLevel.groundDisabled);
            SetTPGround(currentLevel.teleportableGround);
            L_controller.GetComponent<VRTK_Pointer>().enableTeleport = currentLevel.teleportEnabled;
            SetTeleportVisibility(currentLevel.teleportEnabled);
            nadir.SetActive(currentLevel.panoramaLevel);
            sun.enabled = !currentLevel.seperateLightSource;
            //OVRCamera.GetComponent<OVRCameraRig>().usePerEyeCameras = currentLevel.stereoPanorama;
            Debug.Log(level + " level loaded");
        }
    

    private void ResetToStartMode()
    {
        player.transform.localScale = new Vector3(1, 1, 1);
        if (toverstaf)
        {
            toverstaf.ResetPosition();
        }
        if (LDrumStick)
        {
            LDrumStick.ResetPosition();
        }
        if (RDrumStick)
        {
            RDrumStick.ResetPosition();
        }
    }

    public void ResetPos()
    {
        player.transform.position = playerStartPos;
        playerAvatar.transform.position = playerAvatarStartPos;
        OVRCamera.transform.position = playerAvatarStartPos;
    }

    public void DisableObjects(GameObject objectToDisable)
    {
        objectToDisable.SetActive(false);
    }

    public void EnableObjects(GameObject objectToDisable)
    {
        objectToDisable.SetActive(true);

    }

    public void SetToDark()
    {
        sun.intensity = 0f;
        RenderSettings.skybox = skyboxDarkMat;
        ground.GetComponent<Renderer>().material = groundDark;
        RenderSettings.fogColor = fogDark;
        RenderSettings.ambientIntensity = .3f;
    }

    public void SetToLight()
    {
        RenderSettings.fogColor = fogLight;
        sun.intensity = 1f;
        RenderSettings.skybox = skyboxMat;
        ground.GetComponent<Renderer>().material = groundWhite;
        RenderSettings.ambientIntensity = 1f;
    }

    private int FindElementByName(string lookingfor, Level[] array)
    {
        int levelIndex = 0;
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i].level.name == lookingfor)
            {
                levelIndex = i;
            }
        }
        return levelIndex;
    }
    private int FindElementByName(string lookingfor, GameObject[] array)
    {
        int levelIndex = 0;
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i].name == lookingfor)
            {
                levelIndex = i;
            }
        }
        return levelIndex;
    }

    private int FindElementByName(string lookingfor, List<Level> array)
    {
        int levelIndex = -1;
        for (int i = 0; i < array.Count; i++)
        {
            if (array[i].name == lookingfor)
            {
                levelIndex = i;
                Debug.Log("level:" + array[i].name + " - already visited");
            }
        }
        return levelIndex;
    }

    public void FogOnOff(bool fogOn)
    {
        RenderSettings.fog = fogOn;
        RenderSettings.fogStartDistance = 20f;
        RenderSettings.fogEndDistance = 90f;
    }

    public void SetFogTo()
    {
        RenderSettings.fogStartDistance = 120f;
        RenderSettings.fogEndDistance = 300f;
    }

    public void SwitchToPanel()
    {
        foreach (GameObject panel in VRMenuPanels)
        {
            panel.SetActive(false);
        }
        VRMenuPanels[0].SetActive(true);
    }

    public void SwitchToPanel(string panelToSwitchTo)
    {
        foreach (GameObject panel in VRMenuPanels)
        {
            panel.SetActive(false);
        }
        if (VRMenuPanels[FindElementByName(panelToSwitchTo, VRMenuPanels)] != null && panelToSwitchTo != "")
        {
            VRMenuPanels[FindElementByName(panelToSwitchTo, VRMenuPanels)].SetActive(true);
        }
    }

    public void BackToPrevious(object sender, ControllerInteractionEventArgs e)
    {
        if (currentLevel.name != "tutorial" && previouslevels[previouslevels.Count-1].name != "tutorial" && currentLevel.name != "intro" && previouslevels[previouslevels.Count-1].name != "intro")
        {
            if (previouslevels.Count > 0)
            {
                currentLevel = previouslevels[previouslevels.Count - 1];
                previouslevels.RemoveAt(previouslevels.Count - 1);
                Debug.Log("Going back to: " + currentLevel.name);
                SelectLevel(currentLevel.name);
            }
            else
            {
                Debug.Log("Already at first scene");
            }
        }
    }

    public void BackToPrevious()
    {
        if (currentLevel.name != "tutorial" && previouslevels[previouslevels.Count - 1].name != "tutorial" && currentLevel.name != "intro" && previouslevels[previouslevels.Count - 1].name != "intro")
        {
            if (previouslevels.Count > 0)
            {
                currentLevel = previouslevels[previouslevels.Count - 1];
                previouslevels.RemoveAt(previouslevels.Count - 1);
                Debug.Log("Going back to: " + currentLevel.name);
                SelectLevel(currentLevel.name);
            }
            else
            {
                Debug.Log("Already at first scene");
            }
        }
    }

    private void SetTeleportVisibility(bool visible)
    {
        if (visible)
        {
            L_pointer.tracerVisibility = VRTK_BasePointerRenderer.VisibilityStates.OnWhenActive;
            L_pointer.cursorVisibility = VRTK_BasePointerRenderer.VisibilityStates.OnWhenActive;
        }
        else
        {
            L_pointer.tracerVisibility = VRTK_BasePointerRenderer.VisibilityStates.AlwaysOff;
            L_pointer.cursorVisibility = VRTK_BasePointerRenderer.VisibilityStates.AlwaysOff;
        }

    }

    private void SetTPGround(bool tp)
    {
        if (tp)
        {
            ground.layer = 0;
        }
        else
        {
            ground.layer = 10;
        }
    }

    //Deactivate all active scenes
    private void SetScenesInactive()
    {
        for (int i = 0; i < scenes.Length; i++)
        {
            scenes[i].level.SetActive(false);
        }
    }

    //Used in ui menus to trigger sound and haptic feedback
    public void PlaySelectionSound()
    {
        if (!sound.isPlaying)
        {
            if (L_controller.triggerPressed)
            {
                VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(L_controller.gameObject), 1, .1f, .1f);
            }
            if (R_controller.triggerPressed)
            {
                VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(R_controller.gameObject), 1, .1f, .1f);
            }
            sound.Play();
        }
    }

    public void QuitApp()
    {
        Application.Quit();
    }

    public void RestartApp()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

}
