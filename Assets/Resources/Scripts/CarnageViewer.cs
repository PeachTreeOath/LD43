using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


class CarnageViewer : MonoBehaviour
{
    public string[] obituaries = {  "Here lies James Pond. He was 2 days from retirement.",
                                    "Here lies Mary Popcorns. She just had twins, but they probably didn't need a mom.",
                                    "Here lies Tinier Tim. He was on his way to attend Sunday School.",
                                    "Here lies Charlie Purple. He just received the Medal of Honor.",
                                    "Here lies Harly Pottery. She just bought tickets to the \"Beyond Say\" Concert.",
                                    "Here lies Lucky Skyrunner. I pity da foo'.",
                                    "Here lies Jack Parrot. He was stuck between a rock and a hard place.",
                                    "Here lies Donkey Hotay. He wouldn't harm a fly.",
                                    "Here lies Dr. Susies. Curiosity killed the cat.",
                                    "Here lies Ebony Zerscrew. Back to square one.",
                                    "Here lies Nicholas Nickelback. You can't judge a book by its cover.",
                                    "Here lies Elvis Parsley. Elvis has left the building.",
                                    "Here lies Olive R. Twister. He's probably not playing possum.",
                                    "Here lies Chris Toferobin. Who shot the sheriff?",
                                    "Here lies Tom Lawyer. Unleash the beast.",
                                    "Here lies Peter Gryffindor. He was always second best in the family (of two).",
                                    "Here lies Curious Jordan. He just submitted a Game Jam entry.",
                                    "Here lies Frogo Froggins. He was actively committing a felony.",
                                    "Here lies Fizz Lightsecond. He just saw the light at the end of the tunnel.",
                                    "Here lies Peter Bunny. He found love in his final moments.",
                                    "Here lies Robe Hoody. He always paid his taxes.",
                                    "Here lies Tuesday Atoms. She did not live to see another day.",
                                    "Here lies Jessica Bunny. She found their soulmate just the other day.",
                                    "Here lies Rip Car Wink L. He was fit as a fiddle.",
                                    "Here lies David Silverfield. He counted his chickens before they hatched.",
                                    "Here lies Dorothy Gal. She got the short end of the stick.",
                                    "Here lies Pippi Longsock. He rolled with the punches one too many times.",
                                    "Here lies Dr. Dolot. He died doing what he loved.",
                                    "Here lies King Yolomon. It's great to go out in a blaze of glory."};
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
            vp.crashedVehicles[i].GetComponent<VehicleController>().enabled = false;
            //Debug.Log("Set carnage velocity for " + vp.crashedVehicles[i].name);
            GameObject oText = Instantiate(ResourceLoader.instance.obituaryText) as GameObject;
            oText.GetComponent<TextMeshPro>().SetText(obituaries[Random.Range(0, obituaries.Length)]);
            oText.transform.position = vp.crashedVehicles[i].transform.position + Vector3.up * bounds * 1.5f + Vector3.back;
            oText.AddComponent<ObjectFollower>().target = vp.crashedVehicles[i].transform;
        }
    }
}

