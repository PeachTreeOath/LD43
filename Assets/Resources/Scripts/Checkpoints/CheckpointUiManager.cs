using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CheckpointUiManager : MonoBehaviour {

    [SerializeField]
    private GridLayoutGroup layoutParent;

    [SerializeField]
    private GameObject uiCardTemplate;

    [SerializeField]
    private GameObject blankCard; //filler of empty slots, TBD

    private List<CheckpointCard> currentCards;
    private List<GameObject> currentUiCards;

    private int maxCards = 4; //limited by UI space

	// Use this for initialization
	void Start () {
        if (layoutParent == null) {
            Debug.LogError("No layout group specified");
        }
        currentCards = new List<CheckpointCard>();
        currentUiCards = new List<GameObject>();
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// Remove all cards from the current deck.
    /// This should be called before populating the deck
    /// with new cards
    /// </summary>
    public void clearCards() {
        Debug.Log("Clearing UI cards");
        int numBabies = layoutParent.transform.childCount;
        for (int k = numBabies - 1; k >= 0; k--) {
            GameObject child = layoutParent.transform.GetChild(k).gameObject;
            child.transform.SetParent(null);
            Destroy(child);
        }
        currentCards.Clear();
        currentUiCards.Clear();
    }

    /// <summary>
    /// Add cards to the deck. If not enough cards exist to fill
    /// the deck, empty ones will be added
    /// </summary>
    /// <param name="newCards"></param>
    public void addCards(List<CheckpointCard> newCards) {
        int dispIdx = 0;
        foreach (CheckpointCard cc in newCards) {
            GameObject uiCard = createUiCard(cc);
            currentCards.Add(cc);
            currentUiCards.Add(uiCard);
            displayCard(uiCard, dispIdx);
            dispIdx++;
        }
        for (int i = 0; i < (maxCards - currentCards.Count); i++) {
            GameObject blank = createUiCardBlank();
            displayCard(blank, dispIdx);
        }
    }


    /// <summary>
    /// Create the UI display for a checkpoint card.
    /// </summary>
    /// <param name="cc"></param>
    /// <returns></returns>
    private GameObject createUiCard(CheckpointCard cc) {
        GameObject template = Instantiate(uiCardTemplate, Vector2.zero, Quaternion.identity);
        //template expects tags for each item to fill in
        Text itemText = template.GetComponentsInChildren<Text>().Where(r => r.tag == "ItemText").ToArray()[0]; //seems efficient
        if (itemText == null) {
            Debug.LogError("Card does not have Text with correct tag");
        } else {
            itemText.text = cc.getText();
        }

        SpriteRenderer srfg = template.GetComponentsInChildren<SpriteRenderer>()
            .Where(r => r.tag == "ItemSprite").ToArray()[0];
        if (srfg == null) {
            Debug.LogError("Card does not have SpriteRenderer for foreground with correct tag");
        } else {
            srfg.sprite = cc.getItemSprite();
        }

        SpriteRenderer srbg = template.GetComponentsInChildren<SpriteRenderer>()
            .Where(r => r.tag == "ItemBackgroundSprite").ToArray()[0];
        if (srbg == null) {
            Debug.LogError("Card does not have SpriteRenderer for background with correct tag");
        } else {
            srbg.sprite = cc.getBackgroundSprite();
        }

        return template;
    }

    private GameObject createUiCardBlank() {
        GameObject blank = Instantiate(blankCard, Vector2.zero, Quaternion.identity);
        blank.name = "blank card";
        return blank;
    }

    private void displayCard(GameObject uiCard, int displayIdx) {
        Debug.Log("Display card " + uiCard.gameObject.name + " at index " + displayIdx);
        uiCard.transform.SetParent(layoutParent.transform); //will this work?!
    }




}
