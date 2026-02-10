using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    [Header("Límites de la Cámara")]
    public float minX = -0.0556f; 
    public float maxX = 178f; 
    public float minY = -10f; 
    public float maxY = 10f; 

    void LateUpdate()
    {
        if (target == null) return;

        float targetX = target.position.x + offset.x;
        float targetY = target.position.y + offset.y;

        float clampedX = Mathf.Clamp(targetX, minX, maxX);
        float clampedY = Mathf.Clamp(targetY, minY, maxY);

        Vector3 desiredPosition = new Vector3(clampedX, clampedY, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
    }
}