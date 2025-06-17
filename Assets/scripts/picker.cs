using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class picker : MonoBehaviour
{
    [Header("生成设置")]
    // 【修改】更新注释，让它更清晰
    public GameObject[] prefabs; // (0:得分, 1:双倍, 2:回血, 3:高跷道具，5：磁铁)
    public GameObject[] prefabs_damage; // (0:地刺, 1:低障碍物，)
    public float spawnInterval = 0.5f;

    [Header("跑道设置")]
    public float laneDistance = 2.5f; 

    [Header("生成位置参数")]
    public float spawnAheadDistance = 30f;
    public float spawnZRange = 1f;

    private GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player"); 
        if (player == null)
        {
            Debug.LogError("在场景中找不到标签为 'Player' 的玩家对象！请检查玩家的Tag。");
            return;
        }
        InvokeRepeating("SpawnItem", 0.5f, spawnInterval);
    }

    void SpawnItem()
    {
        if (player == null) return;

        // --- 1. 按概率选择一个要生成的预制体 ---
        GameObject prefabToSpawn;
        GameObject prefabToSpawn_1;
        int randomValue = Random.Range(0, 100);
        int randomValue_1 = Random.Range(0, 100);
        // prefabToSpawn 道具  prefabToSpawn  障碍物

        // 【修改】调整生成概率以包含新物品
        if (randomValue < 70) // 70% 的概率生成 prefabs[0] (得分)
        {
            prefabToSpawn = prefabs[0];
        }
        else if (randomValue < 75) // 5% 的概率生成 prefabs[1] (双倍)
        {
            prefabToSpawn = prefabs[1];
        }
        else if (randomValue < 85) // 10% 的概率生成 prefabs[2] (回血)
        {
            prefabToSpawn = prefabs[2];
        }
        else if (randomValue < 95) // 10% 的概率生成 prefabs[3] (高跷道具)
        {
            // 确保 prefabs 数组足够大
            if (prefabs.Length > 3) prefabToSpawn = prefabs[3];
            else prefabToSpawn = prefabs[0]; // 如果没设置，就生成金币作为后备
        }
        //else if (randomValue < 95) // 5% 的概率生成 prefabs[4] (低障碍物)
        //{
        //    // 确保 prefabs 数组足够大
        //    if (prefabs.Length > 4) prefabToSpawn = prefabs[4];
        //    else prefabToSpawn = prefabs[2]; // 如果没设置，就生成高障碍物
        //}
        else  // 5% 的概率生成 prefabs[5] (磁铁)
        {
            prefabToSpawn = prefabs[5];
        }
        //else // 10% 的概率生成 prefabs[2] (高障碍物)
        //{
        //    prefabToSpawn = prefabs[2];
        //}

        if (randomValue_1 < 70) // 70% 的概率生成 prefabs_d[0] (地刺)
        {
            prefabToSpawn_1 = prefabs_damage[0];
        }
        //else if (randomValue_1 < 30) // 15% 的概率生成 prefabs_d[1] (矮墙)
        //{
        //    prefabToSpawn_1 = prefabs_damage[1];
        //}
        else  // 30% 的概率生成 prefabs_d[1] (矮墙)
        {
            prefabToSpawn_1 = prefabs_damage[1];
        }

        // --- 2. 随机选择一条跑道 ---
        int randomLaneIndex = Random.Range(0, 3);
        int randomLaneIndex_1 = Random.Range(0, 3);

        // --- 3. 计算生成位置 ---
        float spawnX = (randomLaneIndex - 1) * laneDistance;
        float spawnZ = player.transform.position.z + spawnAheadDistance + Random.Range(-spawnZRange / 2, spawnZRange / 2);
        float spawnX_1 = (randomLaneIndex_1 - 1) * laneDistance;
        float spawnZ_1 = player.transform.position.z + spawnAheadDistance + 30f + Random.Range(-spawnZRange / 2, spawnZRange / 2);

        // 【修改】让生成位置的Y轴更智能
        // 假设所有道具和障碍物的中心点都在模型底部
        float spawnY = 0.5f; // 默认生成高度
        if (prefabToSpawn.CompareTag("LowObstacle")) {
            spawnY = 0.5f; // 低障碍物的生成高度
        } else if (prefabToSpawn.CompareTag("damage")) {
            spawnY = 0.5f; // 高障碍物可以高一点
        }

        float spawnY_1 = 0.5f; // 默认生成高度
        if (prefabToSpawn_1.CompareTag("LowObstacle"))
        {
            spawnY_1 = 0.5f; // 低障碍物的生成高度
        }
        else if (prefabToSpawn_1.CompareTag("damage"))
        {
            spawnY_1 = -31f; // 高障碍物可以高一点
        }

        Vector3 spawnPosition = new Vector3(spawnX, spawnY, spawnZ);
        Vector3 spawnPosition_1 = new Vector3(spawnX_1, spawnY_1, spawnZ_1);


        // --- 4. 实例化预制体 ---
        Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        Instantiate(prefabToSpawn_1, spawnPosition_1, Quaternion.identity);
    }
}