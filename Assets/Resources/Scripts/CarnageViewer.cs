using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


class CarnageViewer : MonoBehaviour
{
    public string[] obituaries = {
        "Here lies James Pond. He was 2 days from retirement.",
        "Here lies Mother Geese. She just had twins, but they probably didn't need a mom.",
        "Here lies Tinier Tim. He was on his way to attend Sunday School.",
        "Here lies Charlie Purple. He just received the Medal of Honor.",
        "Here lies Harly Pottery. She just bought tickets to the \"Beyond Say\" Concert.",
        "Here lies Lucky Skyrunner. I pity da foo'.",
        "Here lies Jack Parrot. Dead men tell no tales.",
        "Here lies Donkey Hotay. He followed the code ov chivalry to the end.",
        "Here lies Dr. Susie. Curiosity killed the cat.",
        "Here lies Ebony Zerscrew. Back to square one.",
        "Here lies Nicholas Nickelback. You can't judge a book by its cover.",
        "Here lies Elvis Parsley. Elvis has left the building.",
        "Here lies Olive R. Twister. He's probably not playing possum.",
        "Here lies Chris Toferobin. Who shot the sheriff?",
        "Here lies Tom Lawyer. Unleash the beast.",
        "Here lies Peter Gryffindor. He was always second best in the family (of two).",
        "Here lies Curious Jordan. He just submitted a Game Jam entry.",
        "Here lies Frogo Froggins. He was actively committing a felony.",
        "Here lies Fizz Lightsecond. He saw the light at the end of the tunnel.",
        "Here lies Peter Bunny. He found love in his final moments.",
        "Here lies Robe Hoody. He always paid his taxes.",
        "Here lies Tuesday Adams. She did not live to see another day.",
        "Here lies Jessica Bunny. She found her soulmate just the other day.",
        "Here lies Rip Car Wink L. He was fit as a fiddle.",
        "Here lies David Silverfield. He counted his chickens before they hatched.",
        "Here lies Dorothy Gal. She got the short end of the stick.",
        "Here lies Pippi Longsock. He rolled with the punches one too many times.",
        "Here lies Dr. Dolot. He died doing what he loved.",
        "Here lies King Yolomon. It's great to go out in a blaze of glory.",
        "Here lies Little Miss Muppet. She enjoyed her curds and whey.",
        "Here lies Mary Contrary. She lovingly tended to her garden everyday.",
        "Here lies Lumpty Gumpty. He had the respect of the king and all his horses and men.",
        "Here lies Jack Nimble. He was quick, but not quick enough.",
        "Here lies That Guy. Profound sadness.",
        "Here lies Little Piggy. He was on his way home crying \"Wee, wee, wee!\"",
        "Here lies Old Man. It was raining and pouring.",
        "Here lies Hankey Poodle. He loved his feathered hat.",
        "Here lies Rice Crispy. In his final moments he snapped, crackled, and popped.",
        "Here lies Po Beep. She lost her sheep.",
        "Here lies Pat Bakerman. He was baking a cake as fast as he could.",
        "Here lies Rosy Posy. Ashes! Ashes! She fell down.", 
        "Here lies Mance Armstrength.  Now he doesn't even lift."
    };

    VehiclePool vp;
    float scrollMod = .1f;
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
            GameObject fire = Instantiate(ResourceLoader.instance.burningFireFab, vp.crashedVehicles[i].transform.position + Vector3.back, Quaternion.identity);
            fire.AddComponent<ObjectFollower>().target = vp.crashedVehicles[i].transform;

            vp.crashedVehicles[i].GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
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

