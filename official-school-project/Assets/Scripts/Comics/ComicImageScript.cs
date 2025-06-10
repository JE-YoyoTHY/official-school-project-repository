using System.Collections;
using System.Collections.Generic;
//using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

public class ComicImageScript : MonoBehaviour
{
    public ComicPageScript page;

    private Image image;

    [SerializeField] private float hideOpacity;

    [Header("Fade In")]
    [SerializeField] private float fadeInTime;
    [SerializeField] private float fullOpacity; // 0 - 1
    private Coroutine fadeInCoroutine;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    // Start is called before the first frame update
    void Start()
    {
        page = transform.parent.GetComponent<ComicPageScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void showImage()
    {
        gameObject.SetActive(true);

        Color color = image.color;
        color.a = 0f;
        image.color = color;

        fadeInCoroutine = StartCoroutine(imageFadeIn());
    }

    public void hideImage()
    {
        Color color = image.color;
        color.a = hideOpacity;
        image.color = color;
    }

    private IEnumerator imageFadeIn()
    {
        float t = fadeInTime;
        Color color = image.color;

        while (t > 0)
        {
            color.a = Mathf.Lerp(0, fullOpacity, (1 - t) / fadeInTime);
            image.color = color;

            t -= Time.unscaledDeltaTime;

            yield return null;
        }

        imageFadeInEnd();
    }

    public void imageFadeInEnd()
    {
        Color color = image.color;
        color.a = fullOpacity;
        image.color = color;

        if(fadeInCoroutine != null) StopCoroutine(fadeInCoroutine);

        
        page.isImageFadingIn = false;
        
    }
}
