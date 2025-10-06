using UnityEngine;
using UnityEngine.UI;

public class CollectionProgressBar : MonoBehaviour
{
    [Header("UI组件")]
    public Slider progressSlider;         // 进度条Slider
    public Text progressText;             // 进度文本（可选）
    public Image fillImage;               // 填充图片

    [Header("视觉效果")]
    public Color fillColor = Color.green; // 填充颜色
    public bool showPercentage = true;    // 是否显示百分比

    void Start()
    {
        // 初始化UI
        if (fillImage != null)
        {
            fillImage.color = fillColor;
        }

        if (progressSlider != null)
        {
            progressSlider.value = 0;
        }
    }

    // 设置最大值
    public void SetMaxValue(float maxValue)
    {
        if (progressSlider != null)
        {
            progressSlider.maxValue = maxValue;
        }
    }

    // 更新进度
    public void UpdateProgress(float currentValue)
    {
        if (progressSlider != null)
        {
            progressSlider.value = currentValue;
        }

        // 更新进度文本
        if (progressText != null)
        {
            if (showPercentage && progressSlider != null)
            {
                float percentage = (currentValue / progressSlider.maxValue) * 100f;
                progressText.text = $"{percentage:F0}%";
            }
            else
            {
                progressText.text = $"{currentValue:F0}/{progressSlider.maxValue:F0}";
            }
        }
    }

    // 设置进度条颜色
    public void SetFillColor(Color color)
    {
        if (fillImage != null)
        {
            fillImage.color = color;
        }
    }
}