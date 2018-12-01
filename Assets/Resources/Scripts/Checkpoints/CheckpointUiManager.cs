using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointUiManager : MonoBehaviour {

    [SerializeField]
    private GameObject uiCardTemplate;

    [SerializeField]
    private GameObject blankCard; //filler of empty slots, TBD

    private List<CheckpointCard> currentCards;
    private List<GameObject> currentUiCards;

    private int maxCards = 4; //limited by UI space

	// Use this for initialization
	void Start () {
		
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
        //TODO
        return null;
    }

    private GameObject createUiCardBlank() {
        GameObject blank = Instantiate(blankCard, Vector2.zero, Quaternion.identity);
        blank.name = "blank card";
        return blank;
    }

    private void displayCard(GameObject uiCard, int displayIdx) {

    }




}
