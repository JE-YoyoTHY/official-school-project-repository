using UnityEngine;

public class WaterSpring : MonoBehaviour
{
    public float velocity = 0f;  // 當前速度
    public float force = 0f;     // 施加的力
    public float height;         // 當前高度
    public float targetHeight;   // 目標高度 (靜止水面)
    public float stiffness = 0.02f;  // 彈簧剛度
    public float damping = 0.04f;    // 阻尼 (減少擺動)

    public void Init(float initialHeight)
    {
        targetHeight = initialHeight;
        height = initialHeight;
    }

    public void WaterSpringUpdate()
    {
        float x = height - targetHeight;
        force = -stiffness * x;  // 彈簧公式
        velocity += force;
        velocity *= (1 - damping);  // 阻尼讓波紋逐漸減弱
        height += velocity;
    }
}