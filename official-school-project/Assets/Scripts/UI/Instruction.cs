using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instruction : MonoBehaviour
{
    // string: �ӫ��ܪ��W��; GameObject: ��UI����
    public Dictionary<string, GameObject> currentInstructions {  get; private set; } = new Dictionary<string, GameObject>();
    public List<string> availabelInstructionName {  get; private set; } = new List<string>();
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator showInstruction(string instructionName, float delaySec)
    {
        if (currentInstructions.ContainsKey(instructionName))
        {
            Debug.LogError("�ӫ��ܤw�X�{");
            yield break;
        }

        yield return new WaitForSeconds(delaySec);

        //if (availabelInstructionName)
    }



}
