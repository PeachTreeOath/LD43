using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// HTF does this work?  You ask?  Its easy:
/// This manager goes on a gameobject in the scene. It has a reference to the
/// UI that will display the Checkpoint UI (a Canvas named CheckPointUi).
/// The top level of the Checkpoint canvas (named CheckPointUI) has a CheckpointUiManager
/// which controls how shit is added to the UI. It will find a nested CanvasGroup 
/// called layoutParent.  LayoutParent is a GridLayoutGroup that fits the UI cards in 
/// a grid.
/// responsible for placing the cards in their correct spots on the screen.
///
/// The cards that are displayed are based on a template named CpUiCardTemplate. This
/// template has children to display each piece of data on the card.  These children
/// have gameObject tags that identify what type of data they display.
///
/// The actual data for a card is stored on an empty gameObject with a script
/// called CheckpointCard.  This data is relayed into the ui template when the card
/// is placed.
///
/// The data cards (CheckpointCard) are stored per checkpoint.  A checkpoint will have
/// between 0 and max (4) number of cards associated with it.  When the checkpoint is
/// hit it will load the cards into the UI which will convert the data to a visual.
/// If there are less than max cards to display, the empty spots will be filled with a
/// blank looking card (specific in the CheckpointUiManager)
/// </summary>
public class CheckpointManager : MonoBehaviour {

    [SerializeField]
    private Canvas checkpointUi;

    [SerializeField]
    private List<Checkpoint> checkpoints;

    [SerializeField]
    private int curCheckpoint = -1;

    public GameObject prayerHandsFab;

    private CheckpointUiManager uiManager;
    private bool showingCheckpointUi = false;

    private GraphicRaycaster graycast;
    private EventSystem eventSystem;

	// Use this for initialization
	void Start () {
        if (checkpointUi == null) {
            Debug.LogError("Missing checkpoint UI");
        }
        if (checkpoints == null || checkpoints.Count == 0) {
            Debug.LogError("Missing # of checkpoints");
        }
        uiManager = FindObjectOfType<CheckpointUiManager>();
        if (uiManager == null) {
            Debug.LogError("No CheckpointUiManager in the scene... it should be on a Canvas somewhere");
        }
        graycast = uiManager.GetComponentInParent<GraphicRaycaster>();
        if (graycast == null) {
            Debug.LogError("No GraphicRaycaster in scene... missing canvas er something?");
        }
        eventSystem = FindObjectOfType<EventSystem>();
        if (eventSystem == null) {
            Debug.LogError("No event system in scene");
        }

        hideCheckpointUi();
	}
	
	// Update is called once per frame
	void Update () {
        if (showingCheckpointUi) {
            uiManager.clearHighlights();
            updateWithMousePos();
        }		
	}

    //highlight cards that have the mouse over them
    private void updateWithMousePos() {
        Vector2 mousePos = Input.mousePosition;
        PointerEventData ped = new PointerEventData(eventSystem);
        ped.position = mousePos;
        List<RaycastResult> result = new List<RaycastResult>();

        graycast.Raycast(ped, result);
        foreach (RaycastResult rr in result) {
            if (rr.gameObject.CompareTag("ItemHighlight")) {
                uiManager.highlightCard(rr.gameObject, true);
            }
        }
    }

    //returns true if the checkpoint was hit OK
    //returns false if there wasn't a checkpoint to hit
    public bool hitCheckpoint() {
        bool cpHit = true;
        int cp = ++curCheckpoint;
        Debug.Log("Hit checkpoint " + cp);

        if (cp < 0 || cp >= checkpoints.Count) {
            Debug.LogError("Checkpoint ID not valid: " + cp);
            cpHit = false;
        } else {

            //TODO pause gameplay
            if (cp > 0) {
                showCheckpointUi();
            } else {
                resumeCheckpoint();
            }
        }
        return cpHit;

    }

    /// <summary>
    /// Shows the checkpoint ui for the current checkpoint
    /// </summary>
    private void showCheckpointUi() {
        Time.timeScale = 0;
        showingCheckpointUi = true;
        Debug.Log("Show checkpoint UI");

        Checkpoint cp = checkpoints[curCheckpoint];
        if (cp == null) {
            Debug.LogError("Now why did you think that would work??");
            return;
        }

        //load the cards
        List<CheckpointCard> cards = cp.getCards();
        uiManager.clearCards();
        uiManager.addCards(cards);

        //show the cards
        CanvasGroup cpCanvasGroup = checkpointUi.GetComponent<CanvasGroup>();
        cpCanvasGroup.alpha = 1;
        cpCanvasGroup.interactable = true;
        cpCanvasGroup.blocksRaycasts = true;
    }

    /// <summary>
    /// Hide the canvas for the checkpoint
    /// </summary>
    private void hideCheckpointUi() {
        Time.timeScale = 1;
        showingCheckpointUi = false;
        CanvasGroup cpCanvasGroup = checkpointUi.GetComponent<CanvasGroup>();
        cpCanvasGroup.alpha = 0;
        cpCanvasGroup.interactable = false;
        cpCanvasGroup.blocksRaycasts = false;
    }

    /// <summary>
    /// Should be called when the current checkpoint resume
    /// has been requested. 
    /// </summary>
    public void resumeCheckpoint() {
        Debug.Log("Resume checkpoint");

        hideCheckpointUi();

        //TODO make this great
        dbgLoadUpJesusVanPool(curCheckpoint);
        GainPrayers();
    }

    void GainPrayers()
    {
        VehiclePool vp = GameManager.instance.getVehiclePool();
        int prayerCount = 0;
        Debug.Log("vp.vehicles.Count: " + vp.vehicles.Count);
        for (int i = 0; i < vp.vehicles.Count; i++)
        {
            prayerCount += vp.vehicles[i].gameObject.GetComponent<VehicleStats>().prayerValue;
            for(int p = 0; p < vp.vehicles[i].gameObject.GetComponent<VehicleStats>().prayerValue; p++)
            {
                GameObject ph = Instantiate(prayerHandsFab) as GameObject;
                ph.transform.position = vp.vehicles[i].gameObject.transform.position + new Vector3(Random.Range(0f, 1f),
                                                                                                    Random.Range(0f, 1f), 
                                                                                                    -1);
                ph.transform.SetParent(vp.vehicles[i].gameObject.transform);
            }
        }
    }


    private void dbgLoadUpJesusVanPool(int cpIdx) {
        Debug.Log("Debug Spawn Stuff");
        Checkpoint cp = checkpoints[cpIdx];
        //for now we will just dump out
        //everything that is avaialable
        List<VehicleTypeEnum> vs = cp.getAvailableVehicles();

        VehiclePool vp = GameManager.instance.getVehiclePool();

        foreach (VehicleTypeEnum vte in vs) {
            vp.AddNewVehicle(vte);
        }
        
    }


}
