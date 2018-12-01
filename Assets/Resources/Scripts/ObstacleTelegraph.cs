using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleTelegraph : MonoBehaviour
{

    //Serialized Obstacle Telegraph Fields.
    [SerializeField] private AudioClip AudioClip;
    [SerializeField] public float TelegraphHeight;

	// Use this for initialization
	void Start () {

        //Play audio if clip is present.
	    if (AudioClip != null)
	    {
	        //Play Audio Clip
	        var audioSource = gameObject.AddComponent<AudioSource>();
	        audioSource.PlayOneShot(AudioClip);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
