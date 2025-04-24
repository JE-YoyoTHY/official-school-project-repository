using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeShadeScript : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    [SerializeField] private float fadeOutTime;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void summon(){
        StartCoroutine(fadeOut());
    }

    IEnumerator fadeOut(){
        float t = fadeOutTime;
        while(t > 0){
            if(!LogicScript.instance.isFreeze()){
                t -= Time.deltaTime;
            }
            yield return null;
        }

        Destroy(gameObject);
    }
}
