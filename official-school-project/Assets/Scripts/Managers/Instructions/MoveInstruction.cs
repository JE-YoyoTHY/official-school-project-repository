using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveInstruction : MonoBehaviour
{
    public InstructionManager instruManager;
    private bool isExisted = false;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && isExisted == false)
        {
            isExisted = true;
            instruManager.showInstructionUI("MoveInstruction_Mask");
        }
    }
}
