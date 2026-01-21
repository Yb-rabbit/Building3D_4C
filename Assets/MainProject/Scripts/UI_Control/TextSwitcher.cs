using UnityEngine;
using UnityEngine.UI;

public class TextSwitcher : MonoBehaviour
{
    public Text[] texts; // 将所有文本元素拖拽到这个数组中
    private int currentTextIndex = 0; // 当前显示的文本索引

    void Update()
    {
        // 检测鼠标左键按下
        if (Input.GetMouseButtonDown(0))
        {
            SwitchText();
        }
    }

    void SwitchText()
    {
        // 隐藏当前显示的文本
        if (currentTextIndex < texts.Length)
        {
            texts[currentTextIndex].gameObject.SetActive(false);
        }

        // 切换到下一个文本
        currentTextIndex = (currentTextIndex + 1) % texts.Length;

        // 显示新的文本
        if (currentTextIndex < texts.Length)
        {
            texts[currentTextIndex].gameObject.SetActive(true);
        }
    }

    void Start()
    {
        // 初始化，只显示第一个文本，隐藏其他文本
        for (int i = 0; i < texts.Length; i++)
        {
            texts[i].gameObject.SetActive(i == 0);
        }
    }
}
