using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ChangeScenes : MonoBehaviour
{
    public Button changeSceneButton; // 按钮引用
#if UNITY_EDITOR
    public SceneAsset targetScene; // 目标场景引用
#endif
    public string targetSceneName; // 目标场景名称

    void Start()
    {
#if UNITY_EDITOR
        if (targetScene != null)
        {
            targetSceneName = targetScene.name;
        }
#endif

        if (changeSceneButton != null)
        {
            changeSceneButton.onClick.AddListener(LoadNextScene);
        }
        else
        {
            Debug.LogError("Change Scene Button is not assigned!");
        }
    }

    // 调用这个方法来跳转到目标场景
    public void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            if (Application.CanStreamedLevelBeLoaded(targetSceneName))
            {
                SceneManager.LoadScene(targetSceneName);
            }
            else
            {
                Debug.LogError($"Scene '{targetSceneName}' cannot be loaded. Please check the scene name and ensure it is added to the build settings.");
            }
        }
        else
        {
            Debug.LogError("Target scene name is null or empty!");
        }
    }
}
