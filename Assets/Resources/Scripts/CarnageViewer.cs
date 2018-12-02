using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


class CarnageViewer : MonoBehaviour
{
    public string[] obituaries = { "Johny was 2 days from retirement.",
                                    "Linda just had twins. But they probably didn't need a mom.",
                                    "Was on their way to attend Sunday School.",
                                "Just received the Medal of Honor.",
                                "Just bought tickets to the \"Beyond Say\" Concert.",
                                "I pity da foo'.",
                                "Was stuck between a rock and a hard place.",
                                "Wouldn't harm a fly.",
                                "Curiosity killed the cat.",
                                "Back to square one.",
                                "You can't judge a book by its cover.",
                                "Elvis has left the building.",
                                "Probably not playing possum.",
                                "Who shot the sheriff?",
                                "Unleash the beast.",
                                "Was always second best in the family (of two)."};
    VehiclePool vp;
    float scrollMod = .25f;
    void Start()
    {
        Canvas canv = GameManager.instance.GetCheckPointManager().transform.Find("CheckpointSignCanvas").GetComponent<Canvas>();
        ObstacleController[] obstacles = GameObject.FindObjectsOfType<ObstacleController>();
        for(int i = 0; i < obstacles.Length; i++)
        {
            obstacles[i].enabled = false;
            Rigidbody2D rb = obstacles[i].gameObject.AddComponent<Rigidbody2D>();
            rb.velocity = new Vector2(0, LevelManager.instance.scrollSpeed * scrollMod);
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
        GameManager.instance.GetScroller().scrollSpeed = -GameManager.instance.GetScroller().scrollSpeed * scrollMod;
        GameManager.instance.GetCheckPointManager().enabled = false;
        GameManager.instance.GetObstacleSpawner().enabled = false;
        GameManager.instance.GetPrayerMeter().enabled = false;
        GameManager.instance.enabled = false;
        vp = GameManager.instance.getVehiclePool();
        int firstOffScreen = -1;
        Vector3 basePos = Vector3.zero;
        for (int i = vp.crashedVehicles.Count - 1; i >= 0; i--)
        {
            float bounds = Mathf.Max(vp.crashedVehicles[i].transform.GetChild(0).GetComponent<Renderer>().bounds.extents.x, vp.crashedVehicles[i].transform.GetChild(0).GetComponent<Renderer>().bounds.extents.y);
            if (vp.crashedVehicles[i].transform.position.y <= -Camera.main.orthographicSize - bounds)
            {
                if(firstOffScreen == -1)
                {
                    firstOffScreen = i + 1;
                    basePos = new Vector3(vp.crashedVehicles[i].transform.position.x, (firstOffScreen - i) * -Camera.main.orthographicSize, 0); ;
                }
                vp.crashedVehicles[i].transform.position = basePos + Vector3.down * 4 * (firstOffScreen - i);
            }
            vp.crashedVehicles[i].GetComponent<Rigidbody2D>().velocity = new Vector2(0, LevelManager.instance.scrollSpeed * scrollMod);
            GameObject oText = Instantiate(ResourceLoader.instance.obituaryText) as GameObject;
            oText.GetComponent<TextMeshPro>().SetText(obituaries[Random.Range(0, obituaries.Length)]);
            oText.transform.position = vp.crashedVehicles[i].transform.position + Vector3.up * bounds * 1.5f;
            oText.AddComponent<ObjectFollower>().target = vp.crashedVehicles[i].transform;
        }
    }
}

