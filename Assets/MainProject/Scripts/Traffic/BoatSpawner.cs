using UnityEngine;
using System.Collections;

public class BoatSpawner : MonoBehaviour
{
    [Header("预制体")]
    public GameObject[] BoatPrefabs;

    [Header("生成参数")]
    public float minInterval = 0.5f;
    public float maxInterval = 3f;

    [Header("方向设置")]
    public float spawnRotationY = 0f;

    [Header("性能控制")]
    [Tooltip("场景中允许存在的最大船只总数")]
    public int sceneMaxBoatLimit = 50;

    void Start()
    {
        // 将设置的限制同步给 BoatAI 类的静态变量
        BoatAI.MaxBoatLimit = sceneMaxBoatLimit;
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(waitTime);

            if (BoatAI.TotalBoatCount >= BoatAI.MaxBoatLimit)
            {
                // Debug.Log("船只已达上限，暂停生成");
                continue;
            }

            // 随机选择船型
            if (BoatPrefabs.Length > 0)
            {
                int index = Random.Range(0, BoatPrefabs.Length);
                Instantiate(BoatPrefabs[index], transform.position, Quaternion.Euler(0, spawnRotationY, 0));
            }
        }
    }
}
