using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class ComicPageScript : MonoBehaviour
{
    private Image image;

    public string pageID = "";

    [SerializeField] private ComicPageScript nextPage;
    [SerializeField] private ComicImageScript[] comicImages;
    private int currentImage = 0;
    [HideInInspector] public bool isImageFadingIn = false;

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
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            if (isImageFadingIn)
            {
                comicImages[currentImage].imageFadeInEnd();
            }
            else
            {
                showNextImage();
            }
        }

    }

    [ContextMenu("Show Image")]
    public void showPage()
    {
        gameObject.SetActive(true);
        currentImage = 0;

        Color color = image.color;
        color.a = 0f;
        image.color = color;

        foreach(var image in comicImages)
        {
            image.gameObject.SetActive(false);
        }
        comicImages[0].showImage();
        isImageFadingIn = true;

        fadeInCoroutine = StartCoroutine(fadeIn());
    }

    public void hidePage()
    {
        if (nextPage != null)
        {
            nextPage.showPage();
        }
        else
        {
            LogicScript.instance.unpauseGame();
        }

        gameObject.SetActive(false);
    }

    [ContextMenu("Show Next Image")]
    public void showNextImage()
    {

        comicImages[currentImage].hideImage();        
        
        currentImage++;

        if(currentImage >= comicImages.Length)
        {
            hidePage();
        }
        else
        {
            comicImages[currentImage].showImage();
            isImageFadingIn = true;
        }
    }

    private IEnumerator fadeIn()
    {
        float t = fadeInTime;
        Color color = image.color;

        while(t > 0)
        {
            color.a = Mathf.Lerp(0, fullOpacity, (1 - t) / fadeInTime);
            image.color = color;


            t -= Time.unscaledDeltaTime;

            yield return null;
        }
        color.a = fullOpacity;
        image.color = color;
    }

    #region debug

    public void getImagesInChildren()
    {
        comicImages = new ComicImageScript[transform.childCount];
        for(int i = 0;  i < transform.childCount; i++)
        {
            comicImages[i] = transform.GetChild(i).GetComponent<ComicImageScript>();
        }
    }

    #endregion
}

#if UNITY_EDITOR
[CustomEditor(typeof(ComicPageScript))]
public class ComicPageCustomInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        //DrawDefaultInspector();

        ComicPageScript page = (ComicPageScript)target;

        if (GUILayout.Button("Get Images In Children", GUILayout.Width(180f)))
        {
            page.getImagesInChildren();
        }

        if (GUILayout.Button("Show Page", GUILayout.Width(180f)))
        {
            page.showPage();
        }

        if (GUILayout.Button("Show Next Image", GUILayout.Width(180f)))
        {
            page.showNextImage();
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(page);
        }
    }
}


#endif
