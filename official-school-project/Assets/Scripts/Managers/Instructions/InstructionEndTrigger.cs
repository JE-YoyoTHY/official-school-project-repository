using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionEndTrigger : MonoBehaviour
{
    [Header("Drag")]
    [SerializeField] private GameObject instructionUIPrefab;

    [Header("Read-Only")]
    [SerializeField] private InstructionUIManager instructionUIManager;
    [SerializeField] private bool hadEnded = false;

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

        if (collision.CompareTag("Player") && hadEnded == false)
        {
            print("instruction end trigger entered");

            instructionUIManager.disappearInstructionUI();
        }
    }
}
