using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChangeScenes : MonoBehaviour
{
    public Button changeSceneButton;       // 场景切换按钮
    public string targetSceneName;        // 目标场景名称（运行时使用）
    public Image fadeImage;              // 淡出遮罩（需在场景中设置）
    public float fadeDuration = 1f;       // 淡出持续时间（秒）

    void Start()
    {
        // 绑定按钮点击事件
        if (changeSceneButton != null)
        {
            changeSceneButton.onClick.AddListener(LoadNextScene);
        }
        else
        {
            Debug.LogError("Change Scene Button is not assigned!");
        }

        // 初始化淡出遮罩（初始透明度为0）
        if (fadeImage != null)
        {
            fadeImage.color = new Color(0, 0, 0, 0);
        }
    }

    public void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            if (Application.CanStreamedLevelBeLoaded(targetSceneName))
            {
                StartCoroutine(FadeOutAndLoad());
            }
            else
            {
                Debug.LogError($"Scene '{targetSceneName}' cannot be loaded. Check build settings!");
            }
        }
        else
        {
            Debug.LogError("Target scene name is null or empty!");
        }
    }

    private System.Collections.IEnumerator FadeOutAndLoad()
    {
        float elapsedTime = 0f;
        Color startColor = fadeImage.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 1f); // 目标：全黑

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            fadeImage.color = Color.Lerp(startColor, targetColor, elapsedTime / fadeDuration);
            yield return null;
        }

        SceneManager.LoadScene(targetSceneName);
    }
}
