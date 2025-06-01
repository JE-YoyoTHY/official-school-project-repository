using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public static class SpriteMaterialHandler
{
    private const string errorNoRenderer = "Error: No SpriteRenderer (Component) in the given GameObject.";
    private const string success = "Success";
    public static string changeSpriteMaterial(GameObject targetGameObj, Material newMaterial)
    {
        SpriteRenderer renderer = targetGameObj.GetComponent<SpriteRenderer>();
        if (renderer == null)
        {
            return errorNoRenderer;
        }
        else
        {
            renderer.material = newMaterial;
            return success;
        }
    }
    
}
