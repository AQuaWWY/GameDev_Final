using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NewPlayer : MonoBehaviour
{
    [Header("玩家移动参数")]
    public float laneChangeSpeed = 10f; // 切换跑道的速度
    public float forwardSpeed = 5f; // 向前移动的速度
    public float laneDistance = 2.5f; // 两条跑道之间的距离

    private Transform m_transform;
    private int currentLane = 1; // 当前跑道索引 (0: 左, 1: 中, 2: 右)

    [Header("游戏状态")]
    public int current_score = 0; //分数
    public int current_health = 3; //生命值

    [Header("UI组件")]
    private TextMeshProUGUI rest_text;
    private TextMeshProUGUI score_text;

    [Header("游戏结束设置")]
    public EndPanel endPanelManager; // 在Inspector中拖拽拥有EndPanel脚本的对象到这里
    public string finishLineTag = "FinishLine"; // 定义终点线的标签

    // 初始化
    void Start()
    {
        m_transform = this.transform;

        // 查找UI对象并获取组件
        GameObject restObject = GameObject.Find("health");
        if (restObject != null) rest_text = restObject.GetComponent<TextMeshProUGUI>();

        GameObject scoreObject = GameObject.Find("score");
        if (scoreObject != null) score_text = scoreObject.GetComponent<TextMeshProUGUI>();

        // 初始化UI显示
        UpdateScoreUI();
        UpdateHealthUI();
    }

    // 碰撞检测
    void OnTriggerEnter(Collider coll)
    {
        Debug.Log("发生碰撞，碰撞对象: " + coll.gameObject.name);

        if (coll.gameObject.CompareTag("Item")) // 得分
        {
            current_score++;
            UpdateScoreUI();
            Destroy(coll.gameObject);
        }
        else if (coll.gameObject.CompareTag("Heal")) // 回血
        {
            current_health++;
            UpdateHealthUI();
            Destroy(coll.gameObject);
        }
        else if (coll.gameObject.CompareTag("damage")) // 碰到障碍物
        {
            Debug.Log("障碍物");
            current_health--;
            UpdateHealthUI();
            // 如果有碰撞动画或音效可以在这里触发
        }
        else if (coll.gameObject.CompareTag(finishLineTag)) // 碰到终点线
        {
            Debug.Log("玩家到达终点!");
            if (endPanelManager != null)
            {
                endPanelManager.TriggerEndSequence();
                this.enabled = false; // 禁用玩家移动脚本
            }
            else
            {
                Debug.LogError("到达终点，但 EndPanelManager 未在Player上设置!");
            }
        }
        else
        {
            // 可以为其他类型的碰撞添加逻辑，比如墙壁等
            Debug.Log("未分类的碰撞: " + coll.gameObject.tag);
        }

        // 检查游戏是否失败
        if (current_health <= 0)
        {
            Debug.Log("生命值为0，游戏结束!");
            if (endPanelManager != null)
            {
                endPanelManager.TriggerEndSequence(true); // 传入true表示失败
                this.enabled = false;
            }
        }
    }

    // 每帧更新
    void Update()
    {
        // --- 跑道切换输入检测 ---
        // 按下 'A' 或左箭头键，并且不在最左边的跑道
        if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) && currentLane > 0)
        {
            currentLane--; // 移动到左边一条跑道
        }

        // 按下 'D' 或右箭头键，并且不在最右边的跑道
        if ((Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) && currentLane < 2)
        {
            currentLane++; // 移动到右边一条跑道
        }

        // --- 玩家位置更新 ---
        // 1. 计算目标X轴位置
        // (currentLane - 1) 会将跑道索引 0, 1, 2 映射到 -1, 0, 1
        // 再乘以跑道距离，就得到了目标X坐标
        float targetX = (currentLane - 1) * laneDistance;
        Vector3 targetPosition = new Vector3(targetX, m_transform.position.y, m_transform.position.z);

        // 2. 平滑地更新水平位置 (使用Lerp)
        Vector3 newPosition = m_transform.position;
        newPosition.x = Mathf.Lerp(newPosition.x, targetPosition.x, Time.deltaTime * laneChangeSpeed);
        m_transform.position = newPosition;

        // 3. 持续向前移动
        m_transform.Translate(Vector3.forward * Time.deltaTime * forwardSpeed);
    }
    
    // --- UI更新函数 ---
    void UpdateScoreUI()
    {
        if (score_text != null)
            score_text.text = "Score: " + current_score.ToString();
    }

    void UpdateHealthUI()
    {
        if (rest_text != null)
            rest_text.text = "Health: " + current_health.ToString();
    }
}