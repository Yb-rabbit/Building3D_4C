using UnityEngine;
using UnityEngine.UI;

public class MiniWaterWindowSandbox : MonoBehaviour
{
    [Header("微缩模型组件")]
    public Transform cityWater;      // 城内水体
    public Transform riverWater;     // 江水水体
    public Transform valvePivot;     // 水窗木门轴心
    public ParticleSystem flowParticles; // 水流粒子

    [Header("沙盘参数")]
    public float baseLevel = 0.2f;   // 基础/空水位高度
    public float floodLevel = 2.5f;  // 洪峰水位高度
    public float maxOpenAngle = 75f; // 阀门最大开合度
    public float transitionSpeed = 1.5f; // 水位变化平滑速度

    [Header("UI按钮")]
    public Button btnDrain;      // 排涝按钮
    public Button btnFlood;      // 倒灌按钮
    public Button btnBalance;    // 平衡按钮
    public Button btnAutoPlay;   // 自动演示按钮

    // 内部状态
    private float targetCityLevel;
    private float targetRiverLevel;
    private bool isAutoPlaying = false;
    private float autoTimer = 0f;
    private int currentScenario = 0;

    void Start()
    {
        // 初始化水位
        targetCityLevel = baseLevel;
        targetRiverLevel = baseLevel;
        cityWater.localScale = new Vector3(1, baseLevel, 1);
        riverWater.localScale = new Vector3(1, baseLevel, 1);

        // 绑定按钮事件
        btnDrain.onClick.AddListener(() => SetScenario(1));
        btnFlood.onClick.AddListener(() => SetScenario(2));
        btnBalance.onClick.AddListener(() => SetScenario(3));
        btnAutoPlay.onClick.AddListener(ToggleAutoPlay);
    }

    void Update()
    {
        // 平滑过渡水位
        cityWater.localScale = new Vector3(1, 
            Mathf.Lerp(cityWater.localScale.y, targetCityLevel, Time.deltaTime * transitionSpeed), 1);
        riverWater.localScale = new Vector3(1, 
            Mathf.Lerp(riverWater.localScale.y, targetRiverLevel, Time.deltaTime * transitionSpeed), 1);

        // 执行水窗物理逻辑
        WaterWindowPhysics();

        // 自动演示逻辑
        if (isAutoPlaying)
        {
            autoTimer += Time.deltaTime;
            if (autoTimer > 6f) // 每6秒切换一次情景
            {
                autoTimer = 0f;
                currentScenario++;
                if (currentScenario > 3) currentScenario = 1;
                SetScenario(currentScenario);
            }
        }
    }

    // 核心物理逻辑：计算压差控制阀门
    void WaterWindowPhysics()
    {
        float currentCity = cityWater.localScale.y;
        float currentRiver = riverWater.localScale.y;
        float pressureDiff = currentCity - currentRiver;

        if (pressureDiff > 0.05f)
        {
            // 城内压强 > 江外：开门排水
            float openAmount = Mathf.Clamp01(pressureDiff / floodLevel);
            float targetAngle = maxOpenAngle * openAmount;
            valvePivot.localEulerAngles = new Vector3(0, 0, 
                Mathf.Lerp(valvePivot.localEulerAngles.z, targetAngle, Time.deltaTime * 3f));

            if (!flowParticles.isEmitting) flowParticles.Play();
            var main = flowParticles.main;
            main.startSpeed = 3f * openAmount + 1f; // 压差越大水流越急
        }
        else
        {
            // 江外压强 >= 城内：关门防倒灌
            valvePivot.localEulerAngles = new Vector3(0, 0, 
                Mathf.Lerp(valvePivot.localEulerAngles.z, 0, Time.deltaTime * 5f)); // 关门速度要快一点，体现顶紧的感觉

            if (flowParticles.isEmitting) flowParticles.Stop();
        }
    }

    // 切换情景
    void SetScenario(int id)
    {
        currentScenario = id;
        autoTimer = 0f; // 重置自动播放计时器

        switch (id)
        {
            case 1: // 暴雨排涝
                targetCityLevel = floodLevel;
                targetRiverLevel = baseLevel + 0.3f;
                break;
            case 2: // 江水倒灌
                targetCityLevel = baseLevel + 0.5f;
                targetRiverLevel = floodLevel;
                break;
            case 3: // 水位平衡
                targetCityLevel = 1.2f;
                targetRiverLevel = 1.2f;
                break;
        }
    }

    void ToggleAutoPlay()
    {
        isAutoPlaying = !isAutoPlaying;
        autoTimer = 0f;
        // 这里可以替换按钮的文字颜色，提示是否开启
        btnAutoPlay.GetComponentInChildren<Text>().text = isAutoPlaying ? "停止演示" : "自动演示";
        
        if (isAutoPlaying) SetScenario(1); // 开启时从第一个情景开始
    }
}