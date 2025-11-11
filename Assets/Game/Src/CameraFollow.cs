using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;   // The player
    public float zOffset = -10f;

    void LateUpdate()
    {
        if (target == null) return;

        // Match the player's position
        Vector3 newPos = target.position;
        newPos.z = zOffset; // Keep camera behind the scene
        transform.position = newPos;
    }
}
