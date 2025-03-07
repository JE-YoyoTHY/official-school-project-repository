using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class UIPerforming
{
    public static Vector2 getUIPosFromWorldPos(RectTransform canvasRect, Vector3 worldPos)
    {
        Vector2 UIPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            Camera.main.WorldToScreenPoint(worldPos),
            Camera.main,
            out UIPos
        );
        return UIPos;

    }

    public static Vector3 getWorldPosFromRectTransform(RectTransform rect)
    {
        Vector3 worldPos = rect.position;
        return worldPos;
    }

    public static void setUIPosWithCanvasPos(GameObject ui, Vector2 targetUIPos)
    {
        ui.transform.position = targetUIPos;
    }
    public static void setUIPosWithWorldPos(RectTransform canvasRect, RectTransform uiRect, Vector3 targetWorldPos)

    {
        uiRect.anchoredPosition = getUIPosFromWorldPos(canvasRect, targetWorldPos);
    }

    public static void setImageTransparency(Image targetImage, float alpha)
    {
        // 0 = 完全透明; 1 = 完全不透明, 0 <= alpha <= 1
        if (alpha < 0 || alpha > 1) 
        { 
            Debug.LogError("[setImageTransparency]: alpha out of range, it should be between 0 to 1.");
            return; 
        }

        Color newColor = targetImage.color;
        newColor.a = alpha;

        targetImage.color = newColor;
    }
}
