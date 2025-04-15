using UnityEngine;
using UnityEngine.UI;

public class BackgroundChange : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Sprite background_1_1;
    [SerializeField] private Sprite background_2_1;

    // 玩家參考
    [SerializeField] private Transform player;  // 玩家 Transform

    // 當前背景是否已經切換
    private bool isBackgroundChanged = false;

    void Start()
    {
        // 初始設定：背景為 background_1_1
        if (backgroundImage != null && background_1_1 != null)
        {
            backgroundImage.sprite = background_1_1;
        }
    }

    void FixedUpdate()
    {
        // 當玩家座標為 (207, -70, 0) 時，切換背景
        if (player != null && !isBackgroundChanged)
        {
            if (player.position.x == 207 && player.position.y == -70)
            {
                ChangeBackground();  // 換背景
                isBackgroundChanged = true;  // 防止背景重複切換
            }
        }
    }

    public void ChangeBackground()
    {
        // 更換背景為 background_2_1
        if (backgroundImage != null && background_2_1 != null)
        {
            backgroundImage.sprite = background_2_1;
        }
    }
}
