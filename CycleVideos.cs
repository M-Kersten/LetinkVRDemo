using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

//Class to add a video to the playlist
//Title shows as floating message and audiovolume changes the volume of audiosource during the current video
[System.Serializable]
public class VideoLocation
{
    public string title;
    public VideoClip video;
    public float audioVolume;
}

public class CycleVideos : MonoBehaviour
{
    [Tooltip("Add videos to playlist")]
    public VideoLocation[] videos;
    [Tooltip("Length of videos")]
    public float videoDuration;

    private float timeLeft;
    //index number
    private int i = 0;
    [Tooltip("Floating message script")]
    public UIMessage message;
    [Tooltip("Unity standard video player")]
    private VideoPlayer vp;
    private AudioSource sound;

    void Start()
    {
        vp = GetComponent<VideoPlayer>();
        sound = GetComponent<AudioSource>();
        timeLeft = videoDuration;
    }

    void Update()
    {
        if (isActiveAndEnabled)
        {
            //simple timer to next video
            timeLeft -= Time.deltaTime;
            if (timeLeft < 0)
            {
                NextVideo();
                timeLeft = videoDuration;
            }
        }
        else
        {
            timeLeft = videoDuration;
        }
    }

    public void NextVideo()
    {
        i++;
        if (i > videos.Length - 1)
        {
            i = 0;
        }
        vp.clip = videos[i].video;
        sound.volume = videos[i].audioVolume;
        if (videos[i].title != "")
        {
            message.SetNewText(videos[i].title, true);
        }
    }
}
