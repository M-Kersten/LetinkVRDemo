using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrumStick : MonoBehaviour {
    
    private AudioSource drumhit;
    public AudioSource bekkenHit;

    void Start()
    {
        drumhit = gameObject.GetComponent<AudioSource>();
    }

    //3 distinct drumsounds with small randomness
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "drum")
        {
            Debug.Log("drum hit, sound to be played");
            drumhit.pitch = Random.Range(.9f, 1f);
            drumhit.Play();
        }
        if (other.tag == "drumlaag")
        {
            Debug.Log("drum hit, sound to be played");
            drumhit.pitch = Random.Range(.85f, .9f);
            drumhit.Play();
        }
        if (other.tag == "drumhoog")
        {
            Debug.Log("drum hit, sound to be played");
            drumhit.pitch = Random.Range(1f, 1.1f);
            drumhit.Play();
        }
        else if (other.tag == "bekken")
        {
            Debug.Log("bekken hit, sound to be played");
            bekkenHit.pitch = Random.Range(.9f, 1.1f);
            bekkenHit.Play();
        }
    }

}
