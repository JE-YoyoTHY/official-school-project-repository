using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCameraDisplacement : MonoBehaviour
{
    private Vector3 mousePos;
    [SerializeField] private float posMultiplier;
    void Start()
    {
        
    }


    void Update()
    {
        mousePos = Input.mousePosition;
        print(mousePos);

        transform.position = new Vector3(mousePos.x * posMultiplier, mousePos.y * posMultiplier, transform.position.z) ;
    }
}
