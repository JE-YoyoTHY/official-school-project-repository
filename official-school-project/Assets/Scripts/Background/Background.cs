using UnityEngine;

public class BackgroundFollow : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;

    void FixedUpdate(){
        if (cameraTransform != null){
            transform.position = new Vector3(cameraTransform.position.x, cameraTransform.position.y - 5, transform.position.z);
        }
    }
}
