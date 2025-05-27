using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public float speed = 2f; // ���������ƶ��ٶ�
    public float speed_forword = 2f; // ������ǰ�ƶ��ٶ�
    public Transform m_transform;
    public float edge = 5f;
    public int current_score = 0; //����
    public int current_health = 3; //����ֵ
    GameObject rest;
    GameObject score;
    TextMeshProUGUI rest_text;
    TextMeshProUGUI score_text;
    
    [Header("Game End Settings")]
    public EndPanel endPanelManager; // 在Inspector中拖拽拥有EndPanel脚本的对象到这里
    public string finishLineTag = "FinishLine"; // 定义终点线的标签

    // ��ʼ��
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
        if(coll.gameObject.transform.root.gameObject.tag == "Item") //得分
        {
            current_score++;
            score_text.text = "current score: " + current_score.ToString();
            GameObject collidedWith = coll.gameObject;
            // 销毁
            Destroy(collidedWith);
        }
        else if(coll.gameObject.transform.root.gameObject.tag == "Heal") //回血
        {
            current_health++;
            rest_text.text = "rest: " + current_health.ToString();
            GameObject collidedWith = coll.gameObject;
            // 销毁
            Destroy(collidedWith);
        }
        else if(coll.tag == "damage")
        {
            Debug.Log("障碍物");
            current_health--;
            rest_text.text = "rest: " + current_health.ToString();
        }
        else if (coll.gameObject.transform.root.gameObject.tag == "Road")
        {
            Debug.Log("道路");
        }
        // 碰撞到终点线的逻辑
        else if (coll.gameObject.CompareTag(finishLineTag))
        {
            Debug.Log("Player reached the finish line!");
            if (endPanelManager != null)
            {
                endPanelManager.TriggerEndSequence();
                // (可选) 可以在这里禁用玩家的移动脚本，防止游戏暂停后还能通过某些方式移动
                this.enabled = false; 
            }
            else
            {
                Debug.LogError("Reached finish line, but EndPanelManager is not set on Player!");
            }
        }
        else
        {
            Debug.Log("其他碰撞，待检测");
            Debug.Log(coll);
            current_health--;
            rest_text.text = "rest: " + current_health.ToString();
        }
    }

    // ÿ֡����
    void Update()
    {
        Vector3 pos = m_transform.position;
        // �����ƶ�
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

        // �����ƶ�
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

        // ������ǰ�ƶ�
        m_transform.Translate(Vector3.forward * Time.deltaTime * speed_forword);

        //// ��ǰ�ƶ�
        //if (Input.GetKey(KeyCode.W))
        //{
        //    m_transform.Translate(Vector3.forward * Time.deltaTime * speed);
        //}

        //// ����ƶ�
        //if (Input.GetKey(KeyCode.S))
        //{
        //    m_transform.Translate(Vector3.back * Time.deltaTime * speed);
        //}
    }
}
