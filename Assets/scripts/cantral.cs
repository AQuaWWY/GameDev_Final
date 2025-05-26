using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public float speed = 2f; // 控制左右移动速度
    public float speed_forword = 2f; // 控制向前移动速度
    public Transform m_transform;
    public float edge = 5f;

    // 初始化
    void Start()
    {
        m_transform = this.transform;
    }

    // 每帧更新
    void Update()
    {
        Vector3 pos = m_transform.position;
        // 向左移动
        if (Input.GetKey(KeyCode.A))
        {
            if(pos.x<=-edge)
            {
                pos.x = -edge;
            }
            else
            {
                m_transform.Translate(Vector3.left * Time.deltaTime * speed);
            }
        }

        // 向右移动
        if (Input.GetKey(KeyCode.D))
        {
            if (pos.x >= edge)
            {
                pos.x = edge;
            }
            else
            {
                m_transform.Translate(Vector3.right * Time.deltaTime * speed);
            }
        }

        // 持续向前移动
        m_transform.Translate(Vector3.forward * Time.deltaTime * speed_forword);

        //// 向前移动
        //if (Input.GetKey(KeyCode.W))
        //{
        //    m_transform.Translate(Vector3.forward * Time.deltaTime * speed);
        //}

        //// 向后移动
        //if (Input.GetKey(KeyCode.S))
        //{
        //    m_transform.Translate(Vector3.back * Time.deltaTime * speed);
        //}
    }
}
