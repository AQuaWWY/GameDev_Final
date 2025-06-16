using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewPlayer : MonoBehaviour
{
    [Header("玩家移动参数")]
    public float laneChangeSpeed = 10f;
    public float forwardSpeed = 5f;
    public float laneDistance = 2.5f;

    private Transform m_transform;
    private int currentLane = 1;
    private float targetX;

    [Header("游戏状态")]
    public int current_score = 0;
    public int current_health = 3;

    [Header("高跷设置")]
    public int stiltLevel = 0;
    public float heightPerStilt = 0.5f;
    public float stiltChangeSpeed = 15f;
    public Transform stiltVisual;
    private float targetY;
    private Vector3 initialStiltScale;

    // 【新增】磁铁道具参数
    [Header("磁铁道具")]
    public float magnetDuration = 10f; // 磁铁持续时间
    public float magnetRadius = 8f;   // 磁铁吸引半径
    public float itemAttractionSpeed = 20f; // 道具飞向玩家的速度
    private bool isMagnetActive = false; // 磁铁是否激活
    private float magnetTimer; // 磁铁计时器

    [Header("UI组件")]
    public TextMeshProUGUI rest_text;
    public TextMeshProUGUI score_text;
    public Text stilt_text;

    [Header("游戏结束设置")]
    public EndPanel endPanelManager;
    public string finishLineTag = "FinishLine";

    void Start()
    {
        m_transform = this.transform;
        targetX = m_transform.position.x;
        targetY = m_transform.position.y;

        if (stiltVisual != null)
        {
            initialStiltScale = stiltVisual.localScale;
            stiltVisual.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("请在Inspector中设置玩家的 stiltVisual 对象！");
        }

        UpdateScoreUI();
        UpdateHealthUI();
        UpdateStiltUI();
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.CompareTag("Stilts"))
        {
            SFXManager.Instance.PlaySound("pick");
            stiltLevel++;
            UpdateStiltState();
            Destroy(coll.gameObject);
        }
        else if (coll.gameObject.CompareTag("LowObstacle"))
        {
            if (stiltLevel > 0)
            {
                stiltLevel--;
                UpdateStiltState();
                Destroy(coll.gameObject);
                Debug.Log("高跷抵挡了一次低障碍物！");
            }
            else
            {
                TakeDamage();
                Debug.Log("没有高跷，碰到了低障碍物，受到伤害！");
            }
        }
        else if (coll.gameObject.CompareTag("Item"))
        {
            SFXManager.Instance.PlaySound("pick");
            current_score++;
            UpdateScoreUI();
            Destroy(coll.gameObject);
        }
        else if (coll.gameObject.CompareTag("Item_double"))
        {
            SFXManager.Instance.PlaySound("pick");
            current_score*=2;
            UpdateScoreUI();
            Destroy(coll.gameObject);
        }
        else if (coll.gameObject.CompareTag("Heal"))
        {
            SFXManager.Instance.PlaySound("pick");
            current_health++;
            UpdateHealthUI();
            Destroy(coll.gameObject);
        }
        // 【新增】拾取磁铁道具
        else if (coll.gameObject.CompareTag("Magnet"))
        {
            SFXManager.Instance.PlaySound("pick"); // 可以换个独特的音效
            ActivateMagnet();
            Destroy(coll.gameObject);
        }
        else if (coll.gameObject.CompareTag("damage"))
        {
            TakeDamage();
        }
        else if (coll.gameObject.CompareTag(finishLineTag))
        {
            Debug.Log("玩家到达终点!");
            if (endPanelManager != null)
            {
                endPanelManager.TriggerEndSequence(false);
                this.enabled = false;
            }
            else
            {
                Debug.LogError("到达终点，但 EndPanelManager 未在Player上设置!");
            }
        }
    }

    void TakeDamage()
    {
        SFXManager.Instance.PlaySound("hit");
        Debug.Log("玩家受到伤害！");
        current_health--;
        UpdateHealthUI();

        if (current_health <= 0)
        {
            Debug.Log("生命值为0，游戏结束!");
            if (endPanelManager != null)
            {
                endPanelManager.TriggerEndSequence(true);
                this.enabled = false;
            }
        }
    }

    void Update()
    {
        // --- 跑道切换输入检测 ---
        if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) && currentLane > 0)
        {
            SFXManager.Instance.PlaySound("jump");
            currentLane--;
        }
        if ((Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) && currentLane < 2)
        {
            SFXManager.Instance.PlaySound("jump");
            currentLane++;
        }

        // --- 玩家位置更新 ---
        targetX = (currentLane - 1) * laneDistance;
        Vector3 newPosition = m_transform.position;
        newPosition.x = Mathf.Lerp(newPosition.x, targetX, Time.deltaTime * laneChangeSpeed);
        newPosition.y = Mathf.Lerp(newPosition.y, targetY, Time.deltaTime * stiltChangeSpeed);
        m_transform.position = newPosition;
        m_transform.Translate(Vector3.forward * Time.deltaTime * forwardSpeed);

        // 【新增】在Update中持续处理磁铁效果
        HandleMagnet();
    }
    
    // 【新增】激活磁铁的方法
    void ActivateMagnet()
    {
        isMagnetActive = true;
        magnetTimer = magnetDuration;
        Debug.Log("磁铁已激活！");
        // 在这里可以添加激活磁铁时的视觉效果（如玩家身上出现一个光环）和音效
    }
    
    // 【新增】处理磁铁效果的核心逻辑
    void HandleMagnet()
    {
        // 如果磁铁未激活，直接返回
        if (!isMagnetActive)
        {
            return;
        }

        // 磁铁计时
        magnetTimer -= Time.deltaTime;
        if (magnetTimer <= 0)
        {
            isMagnetActive = false;
            Debug.Log("磁铁效果结束。");
            // 在这里可以移除磁铁的视觉效果
            return;
        }

        // 使用OverlapSphere检测半径内的所有碰撞体
        Collider[] hitColliders = Physics.OverlapSphere(m_transform.position, magnetRadius);
        foreach (var hitCollider in hitColliders)
        {
            // 尝试获取碰撞体上的CollectibleItem脚本
            if (hitCollider.TryGetComponent<CollectibleItem>(out CollectibleItem item))
            {
                // 如果找到了，就命令它飞向玩家
                item.StartFollowingPlayer(m_transform, itemAttractionSpeed);
            }
        }
    }

    void UpdateStiltState()
    {
        targetY = 1f + stiltLevel * heightPerStilt;
        if (stiltLevel > 0)
        {
            stiltVisual.gameObject.SetActive(true);
            float totalStiltHeight = targetY;
            stiltVisual.localScale = new Vector3(initialStiltScale.x, totalStiltHeight, initialStiltScale.z);
        }
        else
        {
            stiltVisual.gameObject.SetActive(false);
        }
        UpdateStiltUI();
    }

    void UpdateScoreUI()
    {
        if (score_text != null) score_text.text = "Score: " + current_score.ToString();
    }
    void UpdateHealthUI()
    {
        if (rest_text != null) rest_text.text = "Health: " + current_health.ToString();
    }
    void UpdateStiltUI()
    {
        if (stilt_text != null) stilt_text.text = "高跷高度: " + stiltLevel.ToString();
    }
}