using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PausePanel : MonoBehaviour
{
    [Header("UI 引用")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Text volumeValueText; // 可选：显示当前音量数值

    [Header("音量设置")]
    [SerializeField] private float defaultVolume = 0.8f;       // 默认音量
    [SerializeField] private float minVolume = 0f;
    [SerializeField] private float maxVolume = 1f;

    // 保存音量到 PlayerPrefs 的 key
    private const string VOLUME_KEY = "GameVolume";

    private void Awake()
    {
        // 初始状态：面板隐藏
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        // 每次打开面板时，同步滑块和实际音量
        float currentVolume = AudioListener.volume;
        volumeSlider.minValue = minVolume;
        volumeSlider.maxValue = maxVolume;
        volumeSlider.value = currentVolume;
        UpdateVolumeText(currentVolume);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 暂停游戏逻辑
        Time.timeScale = 0f;
    }

    private void OnDisable()
    {
        // 恢复游戏逻辑
        Time.timeScale = 1f;
    }

    private void Start()
    {
        // 按钮绑定
        resumeButton.onClick.AddListener(OnResumeClicked);
        quitButton.onClick.AddListener(OnQuitClicked);
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);

        // 初始化音量
        float savedVolume = PlayerPrefs.GetFloat(VOLUME_KEY, defaultVolume);
        AudioListener.volume = savedVolume;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Toggle()
    {
        if (gameObject.activeSelf)
            Hide();
        else
            Show();
    }

    private void OnResumeClicked()
    {
        Hide();
    }

    private void OnQuitClicked()
    {
        // 恢复时间 scale，否则切场景会有问题
        Time.timeScale = 1f;

        // 返回主菜单场景
        SceneManager.LoadScene("Menu1");
    }

    private void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat(VOLUME_KEY, value);
        UpdateVolumeText(value);
    }

    private void UpdateVolumeText(float value)
    {
        if (volumeValueText != null)
        {
            volumeValueText.text = Mathf.RoundToInt(value * 100) + "%";
        }
    }
}
