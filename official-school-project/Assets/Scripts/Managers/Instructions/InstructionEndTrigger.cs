using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionEndTrigger : MonoBehaviour
{
    private InstructionStartTrigger startTrigger;
    private InstructionUIManager uiManager;
    private InstructionStartTrigger.InstructionsEnum m_currentInstruction;
    private bool hadEnded = false;

    // Start is called before the first frame update
    void Start()
    {
        startTrigger = transform.parent.transform.GetChild(0).transform.gameObject.GetComponent<InstructionStartTrigger>();
        uiManager = startTrigger.instructionUIManager;
        m_currentInstruction = startTrigger.currentInstruction;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && hadEnded == false)
        {
            hadEnded = true;
            uiManager.disappearInstructionUI();
        }
    }
}
