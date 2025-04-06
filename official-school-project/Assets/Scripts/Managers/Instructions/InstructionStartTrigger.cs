using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionStartTrigger : MonoBehaviour
{
    public InstructionUIManager instructionUIManager;  // inspector 拖入
    public enum InstructionsEnum
    {
        _None, 
        MoveInstruction, 
        JumpInstruction, 
        ShootFireballInstruction
    }
    [SerializeField] public InstructionsEnum currentInstruction; // 用inspector選擇
    [SerializeField] private bool hadStarted = false;
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
        if (collision.CompareTag("Player") && hadStarted == false)  // 從來沒有出現過
        {
            if (currentInstruction == InstructionsEnum._None)
            {
                Debug.LogError("[InstructionTrigger.OnTriggerEnter2D]: InstructionEnum is _None");
                return;
            }
            else
            {
                instructionUIManager.showInstructionUI(currentInstruction.ToString());
                hadStarted = true;
            }
        }

        /*
        if (collision.CompareTag("Player"))
        {
            if (currentInstruction == InstructionsTypeEnum._None) 
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
            hadExistedInstructionObjsName.Add(existingInstructionObjName);
            
        }
        */

    }
}
