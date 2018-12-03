using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrayerHands : MonoBehaviour {

    public float flairTime;
    public float exitTime;
    public float flairHeight;
    public float flairSpeed;

    float delayTime;
    float timer;
    Vector3 startPos;

    enum PHStateEnum { FLAIR, EXIT }
    PHStateEnum phState;
    delegate void action_t();
    List<action_t> actions;
    Vector3 finalPos;
    float startXOffset;

    // Use this for initialization
    void Start () {
        GameObject meter = GameManager.instance.GetPrayerMeter().GetProgressBar();

        float aspectRatio = Screen.width / Screen.height;
        finalPos = new Vector3(0, meter.transform.position.y / Screen.height, -1);
        finalPos = new Vector3(finalPos.x * Camera.main.orthographicSize * aspectRatio, finalPos.y * Camera.main.orthographicSize, -1);

        delayTime = Random.Range(0, .25f);
        startXOffset = gameObject.transform.parent.position.x - gameObject.transform.position.x;
        actions = new List<action_t>();
        actions.Add(FlairTimer);
        actions.Add(ExitTimer);
        timer = Time.time;
    }
	
	// Update is called once per frame
	void Update () {
        startPos = gameObject.transform.parent.position + Vector3.right * startXOffset;
        actions[(int)phState]();
    }

    void FlairTimer()
    {
        if( Time.time - timer >= flairTime)
        {
            timer = Time.time;
            phState = PHStateEnum.EXIT;
            startPos = gameObject.transform.position;
        }
        else
        {
            gameObject.transform.position = startPos + Vector3.up * flairHeight * Mathf.Sin((Time.time - timer + delayTime) * flairSpeed);
        }
    }

    void ExitTimer()
    {
        gameObject.transform.position = Vector3.Lerp(startPos, finalPos, (Time.time - timer) / exitTime);
        if (Time.time - timer >= exitTime)
        {
            GameManager.instance.GetPrayerMeter().AddPrayer(1);
            Destroy(gameObject);
        }
    }
}
