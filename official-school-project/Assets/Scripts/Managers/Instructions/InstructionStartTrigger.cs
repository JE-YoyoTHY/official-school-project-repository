using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InstructionUI;

public class InstructionStartTrigger : MonoBehaviour
{
    [Header("Drag")]
    [SerializeField] private GameObject instructionUIPrefab;
    [SerializeField] private ActionsEnum shootFireball_OneKey;
    [SerializeField] private ActionsEnum shootFireball_TwoKey_First;
    [SerializeField] private ActionsEnum shootFireball_TwoKey_Second;


    [Header("Read-Only")]
    [SerializeField] InstructionUIManager instructionUIManager;
    [SerializeField] private bool hadStarted = false;
    private List<ActionsEnum> shootFireballActions;
    private void Awake()
    {
        instructionUIManager = transform.parent.transform.parent.GetComponent<InstructionUIManager>();
        shootFireballActions = new List<ActionsEnum>()
        {
            ActionsEnum.UpShootFireball, ActionsEnum.LeftShootFireball, ActionsEnum.RightShootFireball, ActionsEnum.DownShootFireball
        };

        if (shootFireballActions.Contains(shootFireball_OneKey) && shootFireballActions.Contains(shootFireball_TwoKey_First) && shootFireballActions.Contains(shootFireball_TwoKey_Second) == false)
        {
            Debug.LogError("shootFireball actions not set correctly.");
        }
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
            instructionUIPrefab.GetComponent<InstructionUI>().changeAction_ShootFireball_OneKey(shootFireball_OneKey);
            instructionUIPrefab.GetComponent<InstructionUI>().changeAction_ShootFireball_TwoKey(shootFireball_TwoKey_First, shootFireball_TwoKey_Second);
            instructionUIManager.showInstructionUI(instructionUIPrefab.GetComponent<InstructionUI>().getCurrentInstructionType());
        }
    }
}
