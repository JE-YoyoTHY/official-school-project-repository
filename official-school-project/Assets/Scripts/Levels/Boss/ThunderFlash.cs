using System.Collections;
using UnityEngine;

public class ThunderFlash : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    void Start()
    {
        SetAlpha(0f);
    }

    public void Flash()
    {
        StartCoroutine(FlashController());
    }

    private IEnumerator FlashController()
    {
        if (spriteRenderer == null) yield break;

        Color originalColor = spriteRenderer.color;

        SetAlpha(1f);
        yield return new WaitForSeconds(0.05f);
        SetAlpha(0f);
        yield return new WaitForSeconds(0.05f);

        SetAlpha(1f);
        yield return new WaitForSeconds(0.05f);
        SetAlpha(0f);
        yield return new WaitForSeconds(0.05f);    

        SetAlpha(originalColor.a);
    }

    private void SetAlpha(float alpha)
    {
        Color c = spriteRenderer.color;
        c.a = alpha;
        spriteRenderer.color = c;
    }
}
