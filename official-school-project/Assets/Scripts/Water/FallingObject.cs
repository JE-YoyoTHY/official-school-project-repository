using UnityEngine;

public class FallingObject : MonoBehaviour
{
    public float impactStrength = 0.5f;

    void OnCollisionEnter2D(Collision2D collision)
    {
        WatershedController water = collision.gameObject.GetComponent<WatershedController>();
        if (water != null)
        {
            int nearestIndex = FindNearestWavePoint(water);
            water.Splash(nearestIndex, impactStrength);
        }
    }

    int FindNearestWavePoint(WatershedController water)
    {
        float minDist = float.MaxValue;
        int index = 0;

        for (int i = 0; i < water.transform.childCount; i++)
        {
            float dist = Mathf.Abs(transform.position.x - water.transform.GetChild(i).position.x);
            if (dist < minDist)
            {
                minDist = dist;
                index = i;
            }
        }
        return index;
    }
}