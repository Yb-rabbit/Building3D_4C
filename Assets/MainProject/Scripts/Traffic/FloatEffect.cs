using UnityEngine;

public class FloatEffect : MonoBehaviour
{
    [Header("浮动范围")]
    [Tooltip("在初始位置基础上的最大上浮距离")]
    public float maxValue = 2f;
    [Tooltip("在初始位置基础上的最大下沉距离")]
    public float minValue = -2f;

    [Header("运动参数")]
    [Tooltip("浮动速度")]
    public float speed = 2f;

    [Header("控制")]
    public bool isPaused = false;

    private float _time;
    private float _originY;
    private float _mid;
    private float _amp;

    void Start()
    {
        _originY = transform.localPosition.y;
        _mid = (maxValue + minValue) * 0.5f;
        _amp = (maxValue - minValue) * 0.5f;
    }

    void Update()
    {
        if (isPaused) return;

        _time += Time.deltaTime;
        float offset = _mid + _amp * Mathf.Sin(_time * speed);
        offset = Mathf.Clamp(offset, minValue, maxValue);

        transform.localPosition = new Vector3(
            transform.localPosition.x,
            _originY + offset,
            transform.localPosition.z
        );
    }

    public void SetRange(float min, float max)
    {
        minValue = Mathf.Min(min, max);
        maxValue = Mathf.Max(min, max);
        _mid = (maxValue + minValue) * 0.5f;
        _amp = (maxValue - minValue) * 0.5f;
    }
}
