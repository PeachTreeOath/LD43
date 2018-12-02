using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class VehicleController : MonoBehaviour
{
    public enum State { ENTERING_STAGE, DRIVING, SWERVING, CRASHING, CRASHED }
    public State currState;

    [Tooltip("The pool the vehicle belongs to.")]
    public VehiclePool vehiclePool;

    public bool isSelected; // This is how a user selects which vehicle to move

    public VehicleBody vehicleBody; // Contains both sprite and collider and are separate from the actual GameObject

    private Rigidbody2D rbody;
    private BoxCollider2D childCollider;

    public float angledMovementPower;

    public float driftPower;

    public VehicleStats vehicleStats;

    public ParticleSystem sparkEmitter;
    public ParticleSystem fireballEmitter;

    //Cap on how fast the car can move on the x-axis per update
    private bool initialized = false;
    private float maxHSpeedConst = 0.17f;
    private static float WAKE_CONTROL_LOWER_BOUND = 5;
    private static float LANE_CORRECTION_ANGLE_DELTA = 2;
    private static float WAKE_TIME = 1f; // 1 second before waking
    private static float mouseDragDeadZone = 0.3f; // values < this are in the mouse movement deadzone

    private bool isHoldingMouse; //for drag detection

    public bool isSleeping;
    public float nextSleepTime;
    public float nextWakeTime;
    private float timeElapsed;

    private Vector2 sleepVector;
    private float swerve;

    private Vector2 drivingVelocity;

    GameObject lightShaft;

    GameObject caption;
    List<Sprite> captionBubbles = new List<Sprite>();

    private JesusFace face;

    public bool IsCrashed
    {
        get
        {
            switch (currState)
            {
                case State.CRASHING:
                case State.CRASHED:
                    return true;
                default:
                    return false;
            }
        }
    }

    void Awake() {
        currState = State.DRIVING;  // this is temporary...
    }

    // Use this for initialization
    void Start()
    {
        initialized = true;
        rbody = GetComponent<Rigidbody2D>();
        nextSleepTime = GetNextSleepOrWakeTime();
        nextWakeTime = float.PositiveInfinity;
        vehiclePool = GameManager.instance.getVehiclePool();
        face = GameObject.Find("JesusBody").GetComponent<JesusFace>();
        isHoldingMouse = false;

        captionBubbles.AddRange(new List<Sprite>
        {
            ResourceLoader.instance.captionBubble1,
            ResourceLoader.instance.captionBubble2,
            ResourceLoader.instance.captionBubble3,
        });
    }

    // Update is called once per frame
    void Update()
    {
        switch (currState)
        {
            case State.DRIVING:
                UpdateDriving();
                break;

            case State.CRASHING:
                UpdateCrashing();
                break;
        }
    }

    void UpdateCrashing()
    {
        if (Math.Abs(rbody.velocity.x) <= 0.1f)
        {
            StartFatalCrash();
        }

        // crashing cars aren't being driven forward, so they need to return to "scrollspeed"
        if (rbody.velocity.y > -LevelManager.instance.scrollSpeed)
        {
            var newYvel = rbody.velocity.y + LevelManager.instance.CrashingFriction * Time.deltaTime; //TODO do this in fixed time?
            rbody.velocity = new Vector2(rbody.velocity.x, -newYvel);
        }
        else
        {
            //TODO do collisions matter here?
            rbody.velocity = new Vector2(rbody.velocity.x, -LevelManager.instance.scrollSpeed);
        }
    }

    public void setEnteringStage(bool isEntering)
    {
        if (isEntering)
        {
            Debug.Log("SetEnteringStage");
            //note, collider is enabled but may be on a different layer
            //if a miracle is in progress
            currState = State.ENTERING_STAGE;
        }
        else
        {
            resetSleepTime(); //don't let the car fall asleep immediately
            currState = State.DRIVING;
        }
    }

    void UpdateDriving()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed > nextSleepTime && !isSleeping)
        {
            //falling asleep
            onDriverSleep();
            if (isSelected)
            {
                resetWakeTime();
            }
        }
        else if (isSleeping && timeElapsed > nextWakeTime)
        {
            //waking up from sleep
            if (isSelected)
            {
                onDriverWake();
            }
        }

        float hInput = 0;
        float vInput = 0;
        if (isSelected) // Only apply input if vehicle is selected, otherwise just continue with drift logic alone
        {
            float horzAxisInput = 0;
            float vertAxisInput = 0;
            if (isHoldingMouse) {
                //get axis values from relative mouse position (i.e. drag dir)
                horzAxisInput = getMouseHorzInput();
                vertAxisInput = getMouseVertInput();
                //Debug.Log("MouseDrag x=" + horzAxisInput + ", y=" + vertAxisInput);
            }

            float mouseHorzAxisInput = Input.GetAxisRaw("Horizontal");
            float mouseVertAxisInput = Input.GetAxisRaw("Vertical");
            bool hasKeyboardInput = !Mathf.Approximately(mouseHorzAxisInput, 0) || !Mathf.Approximately(mouseVertAxisInput, 0);

            //note keyboard controls will override mouse drag if they are being used at the same time
            if (hasKeyboardInput) {
                horzAxisInput = mouseHorzAxisInput;
                vertAxisInput = mouseVertAxisInput;
                //Debug.Log("keyboardInput x=" + horzAxisInput + ", y=" + vertAxisInput);
            }

            // Feel free to adjust these magic numbers to make the movement feel better, the current
            // numbers are balanced around the default car model
            hInput = vehicleStats.control * 3f * horzAxisInput;
            vInput = vehicleStats.speed * 0.0024f * vertAxisInput;
        }

        // Movement from input
        float rotateDelta;
        if (isSelected)
        {
            rotateDelta = (hInput + (sleepVector.x * vehicleStats.sleepSeverity * .82f) + swerve) * Time.deltaTime;
            vehicleBody.transform.Rotate(Vector3.back, rotateDelta);
        }
        else if (!isSleeping)
        {
            // North is 0 or 360
            // if less than 10 or more than 350, do nothing
            // if less than 180, reduce, if more than 180, increase
            float angle = vehicleBody.transform.eulerAngles.z;
            if (angle < 0 + WAKE_CONTROL_LOWER_BOUND || angle > 360 - WAKE_CONTROL_LOWER_BOUND)
            { // do nothing
                rotateDelta = 0f;
            }
            else if (angle < 180)
            { // reduce angle
                rotateDelta = -1f * LANE_CORRECTION_ANGLE_DELTA;
            }
            else
            { // angle > 180
                rotateDelta = LANE_CORRECTION_ANGLE_DELTA;
            }
            vehicleBody.transform.Rotate(Vector3.forward, rotateDelta);
        }
        else
        {
            rotateDelta = (hInput + (sleepVector.x * vehicleStats.sleepSeverity * .2f) + swerve) * Time.deltaTime;
            vehicleBody.transform.Rotate(Vector3.back, rotateDelta);
        }

        if(Math.Abs(swerve) > 0) {
            var oldSign = Mathf.Sign(swerve);
            swerve -= LevelManager.instance.SwerveDecayPerWeight * vehicleStats.weight * Time.deltaTime * oldSign;
            if(Mathf.Sign(swerve) != oldSign) {
                swerve = 0f;
            }
        }

        // Drift vehicle left/right based on how much rotation applied
        float hDelta = GetHorizontalDeltaFromRotation(vehicleBody.transform.eulerAngles.z);

        Vector2 oldPosition = transform.position;
        Vector2 newPosition = oldPosition + new Vector2(hDelta, (vInput + (sleepVector.y * vehicleStats.sleepSeverity * .01f) * Time.deltaTime));

        rbody.MovePosition(newPosition);

        drivingVelocity = (newPosition - oldPosition) / Time.deltaTime; //TODO how to get this to be the right number?
    }

    //Get a 'digital' x read of the mouse position
    private float getMouseHorzInput() {
        return getMouseBinaryAxis(true);
    }

    //Get a 'digital' y read of the mouse position
    private float getMouseVertInput() {
        return getMouseBinaryAxis(false);
    }

    //Get a 'digital' read of the mouse position verse the current position of this object
    //-1 = -x, 0 = none, +1 = +x
    //-1 = -y, 0 = none, +1 = +y
    //Respects a small deadzone area for zero
    //if isHorz == true the reading is for horizontal position (x)
    //otherwise it is for vertical position (y)
    private float getMouseBinaryAxis(bool isHorz) {
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Debug.Log("mousePos=" + pos + ", myPos=" + gameObject.transform.position);
        float diff;
        if (isHorz) {
            diff = (pos - (Vector2)gameObject.transform.position).x;
        } else {
            diff = (pos - (Vector2)gameObject.transform.position).y;
        }

        if (diff < mouseDragDeadZone && diff > -mouseDragDeadZone) {
            diff = 0;
        }
        float result = 0;
        if (diff < 0) {
            result = -1;
        } else if (diff > 0) {
            result = 1;
        }
        return result;
    }
    protected Vector2 currVelocity
    {
        get { return currState == State.DRIVING ? drivingVelocity : rbody.velocity; }
    }

    public void StartDrifting()
    {
        if (currState != State.DRIVING) return;

        // Only pick from top 6 drift directions - don't want to drift north or south.
        DirectionEnum driftDirection = (DirectionEnum)UnityEngine.Random.Range(0, 6);
        switch (driftDirection)
        {
            case DirectionEnum.NE:
                sleepVector = new Vector2(1, 1);
                break;
            case DirectionEnum.E:
                sleepVector = new Vector2(1, 0);
                break;
            case DirectionEnum.SE:
                sleepVector = new Vector2(1, -1);
                break;
            case DirectionEnum.NW:
                sleepVector = new Vector2(-1, 1);
                break;
            case DirectionEnum.W:
                sleepVector = new Vector2(-1, 0);
                break;
            case DirectionEnum.SW:
                sleepVector = new Vector2(-1, -1);
                break;
        }
    }

    public void OnCollideWithWalls(CollisionInfo info)
    {
        if (!initialized || !enabled) return;

        switch (currState)
        {
            case State.DRIVING:
                if (IsHeadOnCrash(info.normal))
                {
                    StartFatalCrash();
                }
                else if (IsAtCrashSpeed())
                {
                    StartSpinningCrash(info);
                }
                else
                {
                    StartSideSwipeSwerve(info);
                }
                break;

            case State.CRASHING:
                StartFatalCrash();
                break;
        }

    }

    public void OnCollideWithVehicle(CollisionInfo info, VehicleController otherVehicle) {
        if (!initialized) return;

        switch (currState)
        {
            case State.DRIVING:
                Bump(info, otherVehicle);
                break;
        }
    }

    private void Bump(CollisionInfo collisionInfo, VehicleController otherVehicle) {
        if( IsHeadOnCrashWithVehicle(collisionInfo.normal, otherVehicle) ) {
            StartSpinningCrash(collisionInfo);
        } else {
            // fake one-dimensional elastic collision?
            var u1 = currVelocity.x;
            var u2 = otherVehicle.currVelocity.x;
            var m1 = vehicleStats.weight;
            var m2 = otherVehicle.vehicleStats.weight;

            var vNew = (u1 * (m1 - m2) + 2 * m2 * u2) / (m1 + m2);

            //TODO this is crazy...
            //TODO reall wants to be the inverse of the squared angle severity calculation...
            swerve = vNew * -LevelManager.instance.BumpingSwerveRatio;
        }
    }

    private bool IsHeadOnCrash(Vector2 normal)
    {
        var headOnCrashPercentage = Math.Abs(Vector2.Dot(normal, vehicleBody.transform.up));
        return (1 - headOnCrashPercentage) <= LevelManager.instance.headOnCrashThreshold;
    }

    private bool IsHeadOnCrashWithVehicle(Vector2 normal, VehicleController otherVehicle) 
    {
        if (!IsHeadOnCrash(normal)) return false;
      
        var goingFrowardPercentage = Math.Abs(Vector2.Dot(normal, Vector2.up));
        if( (1 - goingFrowardPercentage) <= LevelManager.instance.headOnCrashThreshold) {

            /*
            if(otherVehicle.vehicleStats.weight - vehicleStats.weight > vehicleStats.weight || vehicleStats.weight <= 1.5f) {
                return true;
            } 
            */

            //TODO this should probably not just be copy pasta
            var maxControllableSpeed = vehicleStats.weight * LevelManager.instance.SpeedToWeightCrashingRatio * 0.333f;
            return currVelocity.magnitude > maxControllableSpeed;
        } else {
            return true;
        }
    }

    private bool IsAtCrashSpeed()
    {
        var maxControllableSpeed = vehicleStats.weight * LevelManager.instance.SpeedToWeightCrashingRatio;
        return currVelocity.magnitude > maxControllableSpeed;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (UnityEditor.EditorApplication.isPlaying)
        {
            var maxControllableSpeed = vehicleStats.weight * LevelManager.instance.SpeedToWeightCrashingRatio;
            var dir = currVelocity.normalized;

            Vector3 zFix = Vector3.back * 15f;

            Vector3 origin = rbody.transform.position + zFix;
            Vector3 offset = dir * maxControllableSpeed * 0.25f;
            Vector3 offset2 = currVelocity * 0.25f;
            Vector3 offset3 = Vector2.right * swerve * 0.25f;

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(origin, origin + offset);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(origin, origin + offset2);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(origin, origin + offset3);
        }
    }
#endif

    private void StartFatalCrash()
    {
        vehiclePool.OnVehicleCrash(this, true);
        currState = State.CRASHED;
        showCrashedSprite();

        gameObject.layer = LayerMask.NameToLayer("Terrain");

        rbody.bodyType = RigidbodyType2D.Kinematic;
        rbody.angularVelocity = 0f;
        rbody.constraints = RigidbodyConstraints2D.FreezeRotation;

        rbody.velocity = new Vector2(0, -LevelManager.instance.scrollSpeed);

        face.GotoWinceFace();

        fireballEmitter.gameObject.AddComponent<ObjectFollower>().target = gameObject.transform;
        fireballEmitter.Play();

        //TODO crash sound
        ScreenShake();

        Destroy(caption);
    }

    private void showCrashedSprite() {
        vehicleBody.showCrashedSprite();
    }

    private void ScreenShake()
    {
        GameObject.Find("Main Camera").GetComponent<ScreenShake>().TriggerShake(GameManager.instance.screenShakeDurationMs);
    }

    private void StartSpinningCrash(CollisionInfo collisionInfo)
    {
        vehiclePool.OnVehicleCrash(this, false);
        currState = State.CRASHING;

        rbody.drag = LevelManager.instance.CrashingLinearDrag;
        rbody.angularDrag = LevelManager.instance.CrashingAngularDrag;
        rbody.constraints = RigidbodyConstraints2D.None;

        rbody.angularVelocity = collisionInfo.impulse.magnitude * 300f;
        rbody.angularVelocity = Mathf.Clamp(rbody.angularVelocity, -700, 700);
        rbody.velocity = (collisionInfo.normal * 5f) + new Vector2(0, -LevelManager.instance.scrollSpeed * .01f);

        fireballEmitter.gameObject.AddComponent<ObjectFollower>().target = gameObject.transform;
        fireballEmitter.Play();
        //rbody.velocity = new Vector2(0, -LevelManager.instance.scrollSpeed);
        //Debug.Log("Set crash velocity for " + gameObject.name + " enabled: " + enabled);
    }

    private void StartSideSwipeSwerve(CollisionInfo collisionInfo)
    {
        sparkEmitter.Play();

        var percentOfMaxSwerve = 1 - Mathf.Clamp(vehicleStats.weight / LevelManager.instance.WeightForZeroSwerve, 0, 1f);
        swerve = Mathf.Sign(collisionInfo.normal.x) * (LevelManager.instance.MinSwerve + (LevelManager.instance.MaxSwerve - LevelManager.instance.MinSwerve) * percentOfMaxSwerve);
    }

    private void onDriverSleep()
    {
        isSleeping = true;
        RenderSleepCaption();
        StartDrifting();
    }

    private void resetWakeTime()
    {
        // Schedule next wake
        timeElapsed = 0;
        nextWakeTime = WAKE_TIME;
    }

    private void onDriverWake()
    {
        isSleeping = false;
        Destroy(caption);
        resetSleepTime();
        sleepVector = Vector2.zero;

        //Testing prayer allocation on wake.
        GameManager.instance.GetPrayerMeter().AddPrayer(vehicleStats.prayerValue);
    }

    private void resetSleepTime()
    {
        // Schedule next wake
        timeElapsed = 0;
        nextSleepTime = GetNextSleepOrWakeTime();
    }

    private float GetNextSleepOrWakeTime()
    {
        float ret = 8 - vehicleStats.sleepChance * 2 + UnityEngine.Random.Range(-1, 2); // Choose to sleep randomly from 1-7 seconds
        //Debug.Log("GetNextSleepOrWakeTime: " + ret);
        return ret;
    }

    public void SetSelected(bool selected)
    {
        if (selected && currState == State.ENTERING_STAGE) {
            setEnteringStage(false);
            RemoveMiracles();
        }

        if (selected == isSelected) return;

        isSelected = selected;
        if (isSelected)
        {
            AddLight();
        }
        else
        {
            RemoveLight();
        }
    }

    private void RemoveMiracles() {
        MiracleAnimator ma = GetComponentInChildren<MiracleAnimator>();
        if (ma != null) {
            ma.endMiracle();
        }
    }

    private void AddLight()
    {
        if (lightShaft != null) return;

        lightShaft = Instantiate(GameManager.instance.lightShaftsFab) as GameObject;
        lightShaft.transform.position = gameObject.transform.position + Vector3.up * 1;
        lightShaft.AddComponent<ObjectFollower>().target = gameObject.transform;
    }

    private void RemoveLight()
    {
        if (lightShaft != null)
        {
            Destroy(lightShaft);
        }
    }

    public void OnMouseDown()
    {
        if (!IsCrashed)
        {
            isHoldingMouse = true;
            vehiclePool.SelectVehicle(this);
            resetWakeTime();
            //Debug.Log(Time.time  + " reset timeElapsed nextSleepTime " + nextWakeTime);
        }
    }

    public void OnMouseUp() {
        if (isHoldingMouse) {
            isHoldingMouse = false;
        }
    }

    private float GetHorizontalDeltaFromRotation(float eulerAngle)
    {
        float result = 0;
        if (eulerAngle < 180)
        {
            float angle = 0 - eulerAngle;
            float angleSeverity = Mathf.Pow(angle, 2);
            result = -angleSeverity * angledMovementPower;
            result = Math.Max(result, -maxHSpeedConst);
        }
        else
        {
            float angle = 360 - eulerAngle;
            float angleSeverity = Mathf.Pow(angle, 2);
            result = angleSeverity * angledMovementPower;
            result = Math.Min(result, maxHSpeedConst);
        }
        //Debug.Log(gameObject.name + " hDelta for angle " + eulerAngle + " = " + result);
        return result;
    }

    private void RenderSleepCaption()
    {

        //Select sleep caption.
        var randomIndex = UnityEngine.Random.Range(0, captionBubbles.Count);
        var sleepCaption = ResourceLoader.instance.vehicleSleepCaption;

        //Render Sleep Caption
        caption = Instantiate(sleepCaption, vehicleBody.transform.position, Quaternion.identity, vehicleBody.transform);
        caption.GetComponentInChildren<SpriteRenderer>().sprite = captionBubbles.ElementAt(randomIndex);
    }
}