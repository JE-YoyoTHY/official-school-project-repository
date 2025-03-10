using UnityEngine;

public class CloudParallax : MonoBehaviour
{
    private Transform cameraTransform;
    private Vector3 lastCameraPosition;
    public float parallaxEffect = 0.5f;

    void Start(){
        cameraTransform = Camera.main.transform;
        lastCameraPosition = cameraTransform.position;
    }

    void Update(){
        float deltaX = cameraTransform.position.x - lastCameraPosition.x;
        transform.position += new Vector3(deltaX * parallaxEffect*-0.1f, 0);
        lastCameraPosition = cameraTransform.position;
    }
}