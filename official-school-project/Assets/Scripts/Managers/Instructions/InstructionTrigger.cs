using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionTrigger : MonoBehaviour
{
    public InstructionUIManager instructionUIManager;  // inspector ©ì¤J
    private enum InstructionsEnum
    {
        _None, 
        MoveInstruction, 
        JumpInstruction, 
        ShootFireballInstruction
    }
   [SerializeField] private InstructionsEnum currentInstruction;  // ¥Îinspector¿ï¾Ü
    private string existingInstructionObjName;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        print(instructionUIManager.currentInstructionUIObj);
        if (collision.CompareTag("Player"))
        {
            if (currentInstruction == InstructionsEnum._None) 
            {
                Debug.LogError("[InstructionTrigger.OnTriggerEnter2D]: InstructionEnum is _None");
                return; 
            }

            if (instructionUIManager.currentInstructionUIObj != null)
            {
                Debug.Log("[InstructionTrigger.OnTriggerEnter2D]: There is already an existing instruction UI.");
                return;
            }

            instructionUIManager.showInstructionUI(currentInstruction.ToString());
            existingInstructionObjName = currentInstruction.ToString();
            
        }


    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        instructionUIManager.disappearInstructionUI();
    }
}
