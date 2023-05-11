using UnityEngine;

public class StickToCamera : MonoBehaviour
{
    // Reference to the camera that the object should stick to
    public Camera cameraToStickTo;

    // Offset from the camera position that the object should stick to

    void Update()
    {
        // Get the camera position and rotation
        Quaternion cameraRot = cameraToStickTo.transform.rotation;

        // Set the position and rotation of this object to match the camera
        transform.rotation = cameraRot;
    }
}