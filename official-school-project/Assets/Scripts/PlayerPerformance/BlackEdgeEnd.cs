using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackEdgeEnd : MonoBehaviour
{
    [SerializeField] private BlackEdge topEdge;
    [SerializeField] private BlackEdge bottomEdge;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")){
            topEdge.StartSlideOut();
            bottomEdge.StartSlideOut();
        }
    }
}
