using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

public class picker : MonoBehaviour
{
    public float range_x = 15f;
    public float range_z = 1f;
    public GameObject[] prefabs;
    GameObject obj;

    // Start is called before the first frame update
    void Start()
    {
        obj = GameObject.Find("Temp_man");
        Invoke("SpawnItem", 0.1f);
    }

    void SpawnItem()
    {
        for (int i = 0; i < 1; i++)
        {
            GameObject prefab;
            // 随机选择一个预制体
            int x = Random.Range(0, 100);
            if(x / 10 < 8)
            {
                prefab = prefabs[0];
            }
            else if(x / 10 == 8)
            {
                prefab = prefabs[1];
            }
            else
            {
                prefab = prefabs[2];
            }

            // 随机生成位置
            Vector3 randomPosition = new Vector3(
            Random.Range(-range_x / 2, range_x / 2),
            1,
            Random.Range(-range_z / 2 + obj.transform.position.z + 30f, range_z / 2 + obj.transform.position.z + 30f)
            );

            // 实例化物体
            Instantiate(prefab, randomPosition, Quaternion.identity);
        }
        Invoke("SpawnItem", 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
