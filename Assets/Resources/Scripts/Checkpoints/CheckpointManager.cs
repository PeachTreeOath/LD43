using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

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
/// between 0 and max (2) number of cards associated with it.  When the checkpoint is
/// hit it will load the cards into the UI which will convert the data to a visual.
/// If there are less than max cards to display, the empty spots will be filled with a
/// blank looking card (specific in the CheckpointUiManager).
/// 
/// This class monitors the mouse position and clicks when the checkpoint menu is
/// being displayed.  A click will query the ui for the card that was selected via
/// raycast (and ultimately by gameobject tag name).  The ui card is converted
/// back to a data card and returned to this class.
/// </summary>
public class CheckpointManager : MonoBehaviour
{

    public const string distanceOnSign = "MI";

    public const int oneDigitFontSize = 45;

    public const int twoDigitFontSize = 40;

    public const int threeDigitFontSize = 33;

    [SerializeField]
    private Canvas checkpointUi;

    [SerializeField]
    private Checkpoint checkpoint;


    [SerializeField]
    private int curCheckpoint = -1;

    [SerializeField]
    private MiracleAnimator spawnAnimator;

    public GameObject prayerHandsFab;

    private CheckpointUiManager uiManager;
    private bool showingCheckpointUi = false;

    private GraphicRaycaster graycast;
    private EventSystem eventSystem;

    // Use this for initialization
    void Awake()
    {
        uiManager = FindObjectOfType<CheckpointUiManager>();
        if (uiManager == null)
        {
            Debug.LogError("No CheckpointUiManager in the scene... it should be on a Canvas somewhere");
        }
        graycast = uiManager.GetComponentInParent<GraphicRaycaster>();
        if (graycast == null)
        {
            Debug.LogError("No GraphicRaycaster in scene... missing canvas er something?");
        }
    }

    void Start()
    {
        if (checkpointUi == null)
        {
            Debug.LogError("Missing checkpoint UI");
        }

        eventSystem = FindObjectOfType<EventSystem>();
        if (eventSystem == null)
        {
            Debug.LogError("No event system in scene");
        }

        hideCheckpointUi();
    }

    // Update is called once per frame
    void Update()
    {
        if (showingCheckpointUi)
        {
            Vector2 mousePos = Input.mousePosition;
            if (Input.GetMouseButtonDown(0))
            {
                updateWithMouseClick(mousePos);
            }
            else
            {
                uiManager.clearHighlights();
                updateWithMousePos(mousePos);
            }
        }
    }

    public void UpdateCheckpointSignDistance(float distance)
    {
        TextMeshProUGUI textMesh = GetComponentInChildren<TextMeshProUGUI>();

        // If it's in the thousands it's too high!
        //if (distance / 100 >= 1)
        //{
        //   textMesh.fontSize = threeDigitFontSize;
        //}
        //else if (distance / 10 >= 1)
        //{
        //    textMesh.fontSize = twoDigitFontSize;
        //}
        //else
        //{
        //    textMesh.fontSize = oneDigitFontSize;
        //}

        textMesh.fontSize = threeDigitFontSize;

        textMesh.SetText(distance.ToString("0.00") + " " + distanceOnSign);
    }

    //highlight cards that have the mouse over them
    private void updateWithMousePos(Vector2 mouseScreenPos)
    {
        PointerEventData ped = new PointerEventData(eventSystem);
        ped.position = mouseScreenPos;
        List<RaycastResult> result = new List<RaycastResult>();

        graycast.Raycast(ped, result);
        foreach (RaycastResult rr in result)
        {
            if (rr.gameObject.CompareTag("ItemHighlight"))
            {
                uiManager.highlightCard(rr.gameObject, true);
            }
        }
    }

    //highlight cards that have been clicked
    private void updateWithMouseClick(Vector2 mouseScreenPos)
    {
        PointerEventData ped = new PointerEventData(eventSystem);
        ped.position = mouseScreenPos;
        List<RaycastResult> result = new List<RaycastResult>();

        bool isSelected = false;
        graycast.Raycast(ped, result);
        foreach (RaycastResult rr in result)
        {
            if (rr.gameObject.CompareTag("ItemHighlight"))
            {
                isSelected = uiManager.selectCard(rr.gameObject);
            }
        }

        if (isSelected)
        {
            CheckpointCard cc = uiManager.getSelectedCard();
            Debug.Log("You selected " + cc.gameObject.name);
            activateSelectedCard(cc);
            //TODO should pause for selection display for a second or so
            GameManager.instance.resumeCheckpoint();
        }
    }

    //returns true if the checkpoint was hit OK
    //returns false if there wasn't a checkpoint to hit
    public bool hitCheckpoint()
    {
        bool cpHit = true;
        int cp = ++curCheckpoint;
        Debug.Log("Hit checkpoint " + cp);

        //TODO pause gameplay
        if (cp > 0)
        {
            showCheckpointUi();
        }
        else
        {
            resumeCheckpoint();
        }
        return cpHit;

    }

    /// <summary>
    /// Shows the checkpoint ui for the current checkpoint
    /// </summary>
    private void showCheckpointUi()
    {
        Time.timeScale = 0;
        showingCheckpointUi = true;
        Debug.Log("Show checkpoint UI");

        if (checkpoint == null)
        {
            Debug.LogError("Now why did you think that would work??");
            return;
        }

        //load the cards
        List<CheckpointCard> cards = checkpoint.getCards();
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
    private void hideCheckpointUi()
    {
        Time.timeScale = 1;
        showingCheckpointUi = false;
        CanvasGroup cpCanvasGroup = checkpointUi.GetComponent<CanvasGroup>();
        cpCanvasGroup.alpha = 0;
        cpCanvasGroup.interactable = false;
        cpCanvasGroup.blocksRaycasts = false;
    }

    /// <summary>
    /// Should be called when the current checkpoint resume
    /// has been requested.  If you called this directly instead
    /// of calling GameManager resumeCheckpoint then you are
    /// probably wondering why everything didn't resume.
    /// </summary>
    public void resumeCheckpoint()
    {
        Debug.Log("Resume checkpoint");

        hideCheckpointUi();

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
            for (int p = 0; p < vp.vehicles[i].gameObject.GetComponent<VehicleStats>().prayerValue; p++)
            {
                GameObject ph = Instantiate(prayerHandsFab) as GameObject;
                ph.transform.position = vp.vehicles[i].gameObject.transform.position + new Vector3(Random.Range(0f, 1f),
                                                                                                    Random.Range(0f, 1f),
                                                                                                    -1);
                ph.transform.SetParent(vp.vehicles[i].gameObject.transform);
            }
        }
    }


    public void dbgLoadUpJesusVanPool(Checkpoint dbgThing)
    { //starting func
        Debug.Log("Debug Spawn Stuff");
        //for now we will just dump out
        //everything that is avaialable
        List<VehicleTypeEnum> vs = dbgThing.getAvailableVehicles();

        VehiclePool vp = GameManager.instance.getVehiclePool();

        foreach (VehicleTypeEnum vte in vs)
        {
            vp.AddNewVehicle(vte, null); //TODO starting animation
        }

    }

    private void activateSelectedCard(CheckpointCard cc)
    {
        Debug.Log("Activating card " + cc.gameObject.name);
        VehicleTypeEnum vs = cc.getVehicleType();

        VehiclePool vp = GameManager.instance.getVehiclePool();
        vp.AddNewVehicle(vs, spawnAnimator);

    }


}
