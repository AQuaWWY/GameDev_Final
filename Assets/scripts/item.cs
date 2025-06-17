using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class item : MonoBehaviour
{
    GameObject obj;

    // Start is called before the first frame update
    void Start()
    {
        obj = GameObject.Find("Temp_man");
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.z < obj.transform.position.z - 5f)
        {
            Destroy(this.gameObject);
        }
    }
}
