using UnityEngine;

public class LaneMarker : MonoBehaviour {
    #if UNITY_EDITOR
    public void OnDrawGizmos() {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, 0.1f);
    }
    #endif
}
