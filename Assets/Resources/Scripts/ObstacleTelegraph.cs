using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleTelegraph : MonoBehaviour
{
    public ObstacleTypeEnum obstacleType;
    public GameObject telegraphObject;

    //Serialized Obstacle Telegraph Fields.
    [SerializeField] private AudioClip AudioClip;

    private AudioSource audioSource;
    private float blinkInterval = 0.25f;
    private bool isVisible = true;
    private float timeElapsed;

    // Use this for initialization
    void Start()
    {
        timeElapsed = 0;

        //Play audio if clip is present.
        if (AudioClip != null)
        {
            //Play Audio Clip
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.PlayOneShot(AudioClip);
        }
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed > blinkInterval)
        {
            Debug.Log(gameObject.name + ": Toggling " + telegraphObject + " visibility to " + isVisible);
            isVisible = !isVisible;
            if (telegraphObject != null)
            {
                telegraphObject.GetComponent<SpriteRenderer>().enabled = isVisible;
            }
            if (audioSource != null && isVisible)
            {
                audioSource.PlayOneShot(AudioClip);
            }
            timeElapsed = 0f;
        }
    }

    public void SetTelegraphObject(GameObject telegraph)
    {
        Debug.Log(gameObject.name + ": Setting telegraph object to " + telegraph);
        telegraphObject = telegraph;
    }
}