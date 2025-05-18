using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZeusPhaseTwoAttackPosScript : MonoBehaviour
{
    //private SpriteRenderer spriteRenderer;
    //public int attackIndex;
    private void Awake()
    {
        //spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void showUp(Vector3 size)
    {
        transform.localScale = size;
        GetComponent<SpriteRenderer>().enabled = true;


    }

    public void hideAttack()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }
}
