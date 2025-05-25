using UnityEngine;

public class FollowedByCamera : MonoBehaviour
{
    public Camera cameraToFollow; // La caméra qui va suivre ce cube
    public Vector3 offset = new Vector3(5, 10, -5);
    public float smoothSpeed = 0.125f;

    void LateUpdate()
    {
        if (cameraToFollow == null) return;

        Vector3 desiredPosition = transform.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(cameraToFollow.transform.position, desiredPosition, smoothSpeed);
        cameraToFollow.transform.position = smoothedPosition;

        // On bloque la rotation Y de la caméra
        Vector3 camEuler = cameraToFollow.transform.eulerAngles;
        camEuler.y = 0;
        cameraToFollow.transform.eulerAngles = camEuler;
    }

}
