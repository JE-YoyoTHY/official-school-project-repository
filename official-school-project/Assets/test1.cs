using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test1 : MonoBehaviour
{
    [SerializeField] private Material mat;
    void Start()
    {
        SpriteMaterialHandler.changeSpriteMaterial(gameObject, mat);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
