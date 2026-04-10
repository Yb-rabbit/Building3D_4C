using Cinemachine;
using UnityEngine;

public class FreeLookController : MonoBehaviour
{
    private CinemachineFreeLook freeLook; // 控制旋转
    private CinemachineVirtualCamera virtualCamera; // 控制高度（直接修改Transform）
    private float minHeight = 10f; // 最小高度（防止穿入地下）
    private float maxHeight = 200f; // 最大高度（防止飞出场景）

    void Start()
    {
        // 获取组件（确保Virtual Camera同时有FreeLook和VirtualCamera组件）
        freeLook = GetComponent<CinemachineFreeLook>();
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        if (freeLook == null || virtualCamera == null)
        {
            Debug.LogError("请确保Virtual Camera同时添加了CinemachineFreeLook和CinemachineVirtualCamera组件！");
        }
    }

    void Update()
    {
        if (freeLook == null || virtualCamera == null) return;

        // 1. 滚轮控制高度（直接修改Virtual Camera的y坐标，绕过m_Height）
        float scrollDelta = Input.GetAxis("Mouse ScrollWheel");
        if (scrollDelta != 0)
        {
            Vector3 pos = virtualCamera.transform.position;
            pos.y += scrollDelta * 10f; // 滚轮速度（可调整）
            pos.y = Mathf.Clamp(pos.y, minHeight, maxHeight); // 限制高度范围
            virtualCamera.transform.position = pos;
        }

        // 2. 鼠标移动控制旋转（FreeLook的旋转逻辑，兼容新旧版本）
        if (Input.GetMouseButton(0))
        {
            // 新版Cinemachine（3.x+）：通过Value属性修改
            freeLook.m_XAxis.Value += Input.GetAxis("Mouse X") * freeLook.m_XAxis.m_MaxSpeed;
            freeLook.m_YAxis.Value += Input.GetAxis("Mouse Y") * freeLook.m_YAxis.m_MaxSpeed;
            // 旧版Cinemachine（2.x-）：直接修改（若报错则注释掉新版代码，启用旧版）
            // freeLook.m_XAxis += Input.GetAxis("Mouse X") * freeLook.m_XAxis.m_MaxSpeed;
            // freeLook.m_YAxis += Input.GetAxis("Mouse Y") * freeLook.m_YAxis.m_MaxSpeed;
        }
    }
}
