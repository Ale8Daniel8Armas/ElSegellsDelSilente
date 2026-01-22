using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    [Header("Límites de la Cámara")]
    public float minX = -0.0556f; 
    public float maxX = 178f; 

    void LateUpdate()
    {
        if (target == null) return;

        float targetX = target.position.x + offset.x;

        // Esto impide que la cámara pase de minX o maxX
        float clampedX = Mathf.Clamp(targetX, minX, maxX);

        Vector3 desiredPosition = new Vector3(clampedX, transform.position.y, transform.position.z);
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}