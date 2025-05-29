using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionStartTrigger : MonoBehaviour
{
    [Header("Drag")]
    [SerializeField] private GameObject instructionUIPrefab;   

    [Header("Read-Only")]
    [SerializeField] InstructionUIManager instructionUIManager;
    [SerializeField] private bool hadStarted = false;

    private void Awake()
    {
        instructionUIManager = transform.parent.transform.parent.GetComponent<InstructionUIManager>();
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && hadStarted == false)  // 從來沒有出現過
        {
            hadStarted = true;
            instructionUIManager.showInstructionUI(instructionUIPrefab.GetComponent<InstructionUI>().getCurrentInstructionType());
        }
    }
}
