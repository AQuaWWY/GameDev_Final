using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public float speed = 2f; // 控制左右移动速度
    public float speed_forword = 2f; // 控制向前移动速度
    public Transform m_transform;
    public float edge = 5f;
    public int current_score = 0; //分数
    public int current_health = 3; //生命值
    GameObject rest;
    GameObject score;
    TextMeshProUGUI rest_text;
    TextMeshProUGUI score_text;

    // 初始化
    void Start()
    {
        m_transform = this.transform;
        rest = GameObject.Find("health");
        score = GameObject.Find("score");
        rest_text = rest.GetComponent<TextMeshProUGUI>();
        score_text = score.GetComponent<TextMeshProUGUI>();
        current_score = 0;
        current_health = 3;
    }
    void OnTriggerEnter(Collider coll)
    {
        Debug.Log("发生碰撞");
        if(coll.gameObject.tag == "Item") //得分
        {
            current_score++;
            score_text.text = "current score: " + current_score.ToString();
            GameObject collidedWith = coll.gameObject;
            // 销毁碰撞到的物体
            Destroy(collidedWith);
        }
        else if(coll.gameObject.tag == "Heal") //回血
        {
            current_health++;
            rest_text.text = "rest: " + current_health.ToString();
            GameObject collidedWith = coll.gameObject;
            // 销毁碰撞到的物体
            Destroy(collidedWith);
        }
        else if(coll.gameObject.tag == "damage")
        {
            Debug.Log("碰到障碍物");
            current_health--;
            rest_text.text = "rest: " + current_health.ToString();
        }
        else
        {
            Debug.Log("默认为障碍物");
            current_health--;
            rest_text.text = "rest: " + current_health.ToString();
        }
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
