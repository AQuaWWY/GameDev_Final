using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class picker : MonoBehaviour
{
    [Header("生成设置")]
    public GameObject[] prefabs; // 用于生成的预制体数组 (0:得分, 1:回血, 2:障碍物)
    public float spawnInterval = 0.5f; // 生成物品的时间间隔

    [Header("跑道设置")]
    // !!! 重要: 这个值必须和你的玩家移动脚本中的 laneDistance 完全一样 !!!
    public float laneDistance = 2.5f; 

    [Header("生成位置参数")]
    public float spawnAheadDistance = 30f; // 在玩家前方多远的位置生成
    public float spawnZRange = 1f; // Z轴上的一个小的随机范围，防止物品完全在一条直线上

    private GameObject player; // 玩家对象的引用

    void Start()
    {
        // 使用标签查找玩家，比用名字更可靠
        player = GameObject.FindGameObjectWithTag("Player"); 
        
        if (player == null)
        {
            Debug.LogError("在场景中找不到标签为 'Player' 的玩家对象！请检查玩家的Tag。");
            return; // 如果找不到玩家，则停止后续操作
        }

        // 启动生成循环
        InvokeRepeating("SpawnItem", 0.5f, spawnInterval);
    }

    void SpawnItem()
    {
        if (player == null) return; // 如果玩家不存在，则不生成

        // --- 1. 按概率选择一个要生成的预制体 ---
        GameObject prefabToSpawn;
        int randomValue = Random.Range(0, 100); // 生成0-99的随机数

        if (randomValue < 80) // 80% 的概率生成 prefabs[0] (例如：金币)
        {
            prefabToSpawn = prefabs[0];
        }
        else if (randomValue < 90) // 10% 的概率生成 prefabs[1] (例如：治疗)
        {
            prefabToSpawn = prefabs[1];
        }
        else // 10% 的概率生成 prefabs[2] (例如：障碍物)
        {
            prefabToSpawn = prefabs[2];
        }

        // --- 2. 随机选择一条跑道 ---
        // Random.Range(0, 3) 会返回 0, 1, 或 2，正好对应我们的三条跑道
        int randomLaneIndex = Random.Range(0, 3); // 0: 左跑道, 1: 中跑道, 2: 右跑道

        // --- 3. 计算生成位置 ---
        // 使用和玩家脚本完全相同的公式来计算X坐标
        // (randomLaneIndex - 1) 会将 0,1,2 映射为 -1,0,1
        float spawnX = (randomLaneIndex - 1) * laneDistance;

        // Z坐标：在玩家前方一定距离，并加上一个小的随机偏移
        float spawnZ = player.transform.position.z + spawnAheadDistance + Random.Range(-spawnZRange / 2, spawnZRange / 2);
        
        // 最终的生成位置
        Vector3 spawnPosition = new Vector3(spawnX, 1f, spawnZ); // Y坐标可以根据你的物品大小调整

        // --- 4. 实例化预制体 ---
        Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
    }

    // Update is not needed for this spawner logic
    // void Update() { }
}