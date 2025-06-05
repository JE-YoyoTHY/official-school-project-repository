using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZeusPowerCrystalInstanceScript : MonoBehaviour
{
    private ZeusPowerCrystalManagerScript crystalManager;
    private SpriteRenderer spriteRenderer;

    [SerializeField] private Sprite[] sprites;

    [Header("floating")]
    private float floatDistanceMin = 0.7f; // y
    private float floatDistanceMax = 0.4f; // y
    private float floatTimeMin = 3.5f;
    private float floatTimeMax = 6.0f;
    private float currentFloatAngle; //SHM, using sine to float
    private float floatInitialPosY;

    private float currentFloatTime;
    private float currentFloatDistance;

    //private bool isDestroyed = false;

    // Start is called before the first frame update
    void Start()
    {
        crystalManager = transform.parent.GetComponent<ZeusPowerCrystalManagerScript>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        spriteRenderer.sprite = sprites[Random.Range((int)0, sprites.Length)];

        floatInitialPosY = transform.position.y;

        currentFloatTime = Random.Range(floatTimeMin, floatTimeMax);
        currentFloatDistance = Random.Range(floatDistanceMin, floatDistanceMax);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = new Vector3(transform.position.x, floatInitialPosY + Mathf.Sin(currentFloatAngle * Mathf.Deg2Rad) * currentFloatDistance, transform.position.z);
        
        currentFloatAngle += Time.fixedDeltaTime * (360 / currentFloatTime);
        while (currentFloatAngle > 360)
        {
            currentFloatAngle -= 360;
            
            //random
            currentFloatTime = Random.Range(floatTimeMin, floatTimeMax);
            currentFloatDistance = Random.Range(floatDistanceMin, floatDistanceMax);
        } 
    }

    public void crystalInstanceDestroy()
    {
        crystalManager.crystalDestroy();

        gameObject.SetActive(false);
        GetComponent<ParticleCommonScript>().emitParticle();
    }

    public void resetCrystal()
    {
        gameObject.SetActive(true);
    }
}
