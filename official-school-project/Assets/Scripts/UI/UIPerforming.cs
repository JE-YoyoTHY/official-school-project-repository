using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
