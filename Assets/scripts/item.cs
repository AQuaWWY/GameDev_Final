using System.Collections;
using System.Collections.Generic;
//using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement; // 需要这个来进行场景管理 (如重新加载场景)

public class item : MonoBehaviour
{
    GameObject obj;

    // Start is called before the first frame update
    void Start()
    {
        obj = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.z < obj.transform.position.z - 10f)
        {
            Destroy(this.gameObject);
        }
        Scene s = SceneManager.GetActiveScene();
        if (s.name == "endless")
        {
            if (transform.position.z > obj.transform.position.z + 150f)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
