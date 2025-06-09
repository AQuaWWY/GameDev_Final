using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI; // 如果你想通过代码控制按钮等UI元素

public class NewPlayer : MonoBehaviour
{
    [Header("玩家移动参数")]
    public float laneChangeSpeed = 10f; // 切换跑道的速度
    public float forwardSpeed = 5f; // 向前移动的速度
    public float laneDistance = 2.5f; // 两条跑道之间的距离

    private Transform m_transform;
    private int currentLane = 1; // 当前跑道索引 (0: 左, 1: 中, 2: 右)
    private float targetX; // 【新增】目标X位置，用于平滑移动

    [Header("游戏状态")]
    public int current_score = 0; //分数
    public int current_health = 3; //生命值

    // 【新增】高跷相关参数
    [Header("高跷设置")]
    public int stiltLevel = 0; // 当前高跷的层数
    public float heightPerStilt = 0.5f; // 每层高跷增加的高度
    public float stiltChangeSpeed = 15f; // 升高或降低的速度
    public Transform stiltVisual; // 在Inspector中拖拽玩家脚下的高跷视觉模型
    private float targetY; // 目标Y轴高度
    private Vector3 initialStiltScale; // 高跷模型的初始缩放

    [Header("UI组件")]
    public TextMeshProUGUI rest_text; // 【建议】改为public，在Inspector中拖拽赋值
    public TextMeshProUGUI score_text; // 【建议】改为public，在Inspector中拖拽赋值
    public Text stilt_text; // 【新增】用于显示高跷层数的UI Text

    [Header("游戏结束设置")]
    public EndPanel endPanelManager;
    public string finishLineTag = "FinishLine";

    void Start()
    {
        m_transform = this.transform;
        targetX = m_transform.position.x;
        targetY = m_transform.position.y; // 初始化目标高度

        // 【新增】高跷初始化
        if (stiltVisual != null)
        {
            initialStiltScale = stiltVisual.localScale;
            stiltVisual.gameObject.SetActive(false); // 游戏开始时隐藏高跷
        }
        else
        {
            Debug.LogError("请在Inspector中设置玩家的 stiltVisual 对象！");
        }

        // 初始化UI显示
        UpdateScoreUI();
        UpdateHealthUI();
        UpdateStiltUI(); // 【新增】
    }

    void OnTriggerEnter(Collider coll)
    {
        // --- 拾取高跷道具 ---
        if (coll.gameObject.CompareTag("Stilts"))
        {
            stiltLevel++;
            UpdateStiltState();
            Destroy(coll.gameObject);
        }
        // --- 与低障碍物碰撞 ---
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
        // --- 得分 ---
        else if (coll.gameObject.CompareTag("Item"))
        {
            current_score++;
            UpdateScoreUI();
            Destroy(coll.gameObject);
        }
        // --- 回血 ---
        else if (coll.gameObject.CompareTag("Heal"))
        {
            current_health++;
            UpdateHealthUI();
            Destroy(coll.gameObject);
        }
        // --- 普通障碍物 ---
        else if (coll.gameObject.CompareTag("damage"))
        {
            TakeDamage();
        }
        // --- 【重点修正】终点线逻辑 ---
        else if (coll.gameObject.CompareTag(finishLineTag))
        {
            Debug.Log("玩家到达终点!"); // 添加一个日志方便调试
            if (endPanelManager != null)
            {
                // 调用EndPanel脚本中的方法，并传入false表示这不是失败
                endPanelManager.TriggerEndSequence(false); 
                this.enabled = false; // 禁用玩家移动脚本，让玩家停下来
            }
            else
            {
                Debug.LogError("到达终点，但 EndPanelManager 未在Player上设置!");
            }
        }
    }

    // 【新增】一个统一的受伤函数，方便调用
    void TakeDamage()
    {
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
            currentLane--;
        }
        if ((Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) && currentLane < 2)
        {
            currentLane++;
        }

        // --- 玩家位置更新 ---
        // 1. 计算目标X轴位置
        targetX = (currentLane - 1) * laneDistance;

        // 2. 平滑地更新水平和垂直位置 (使用Lerp)
        Vector3 newPosition = m_transform.position;
        newPosition.x = Mathf.Lerp(newPosition.x, targetX, Time.deltaTime * laneChangeSpeed);
        newPosition.y = Mathf.Lerp(newPosition.y, targetY, Time.deltaTime * stiltChangeSpeed); // 【修改】平滑更新Y轴
        m_transform.position = newPosition;

        // 3. 持续向前移动
        m_transform.Translate(Vector3.forward * Time.deltaTime * forwardSpeed);
    }

    // 【新增】更新高跷状态和视觉效果的函数
    // 【重点修改】这个函数变得更简单了！
    void UpdateStiltState()
    {
        // 1. 计算玩家新的目标Y轴高度 (这部分不变)
        targetY = 1f + stiltLevel * heightPerStilt;

        // 2. 更新高跷视觉模型 (现在是更新 StiltPivot)
        if (stiltLevel > 0)
        {
            stiltVisual.gameObject.SetActive(true);

            // 我们要让高跷的总高度 = 玩家脚下到地面的距离。
            // 在我们的新结构中，玩家的Y坐标就是高跷需要延伸的总高度。
            // 我们只需要设置 StiltPivot 的 Y 轴缩放即可。
            float totalStiltHeight = targetY; 
        
            // 直接设置 StiltPivot 的缩放。因为其子对象(圆柱体)的顶部在轴心点，
            // 所以缩放 StiltPivot 就会让圆柱体从玩家脚下向下变长。
            stiltVisual.localScale = new Vector3(initialStiltScale.x, totalStiltHeight, initialStiltScale.z);
        
            // 【删除】不再需要手动计算和设置 localPosition 了！
            // stiltVisual.localPosition = new Vector3(0, -stiltModelYPos, 0); // 这行代码可以删掉了
        }
        else
        {
            stiltVisual.gameObject.SetActive(false);
        }

        // 3. 更新UI (不变)
        UpdateStiltUI();
    }

    // --- UI更新函数 ---
    void UpdateScoreUI()
    {
        if (score_text != null) score_text.text = "Score: " + current_score.ToString();
    }
    void UpdateHealthUI()
    {
        if (rest_text != null) rest_text.text = "Health: " + current_health.ToString();
    }
    // 【新增】更新高跷UI的函数
    void UpdateStiltUI()
    {
        if (stilt_text != null) stilt_text.text = "高跷高度: " + stiltLevel.ToString();
    }
}