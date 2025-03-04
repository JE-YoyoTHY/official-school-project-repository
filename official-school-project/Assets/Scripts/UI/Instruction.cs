using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instruction : MonoBehaviour
{
    // string: ¸Ó«ü¥Üªº¦WºÙ; GameObject: ¸ÓUIª«¥ó
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
            Debug.LogError("¸Ó«ü¥Ü¤w¥X²{");
            yield break;
        }

        yield return new WaitForSeconds(delaySec);

        //if (availabelInstructionName)
    }



}
