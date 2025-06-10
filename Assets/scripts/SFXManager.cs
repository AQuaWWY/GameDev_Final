using UnityEngine;
using System.Collections.Generic; // 用于使用 List
using System.Linq; // 用于方便地查找列表中的元素

// 用于在 Inspector 面板中显示音效配置
[System.Serializable]
public class SoundEffect
{
    public string name; // 音效的名称，用于调用
    public AudioClip clip; // 音频文件

    [Range(0f, 1f)]
    public float volume = 1.0f; // 音量

    [Range(0.1f, 3f)]
    public float pitch = 1.0f; // 音高
}

public class SFXManager : MonoBehaviour
{
    // ----------------- 单例模式 -----------------
    // 静态实例，允许其他任何脚本通过 SFXManager.Instance 来访问
    public static SFXManager Instance { get; private set; }

    // ----------------- 可配置字段 -----------------
    [SerializeField]
    private List<SoundEffect> soundEffects; // 在 Inspector 中配置的音效列表

    // ----------------- 私有组件 -----------------
    private AudioSource audioSource; // 用于播放音效的 AudioSource 组件

    void Awake()
    {
        // --- 单例模式实现 ---
        if (Instance == null)
        {
            Instance = this;
            // 确保在加载新场景时，此游戏对象不会被销毁
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            // 如果实例已存在，则销毁这个重复的实例
            Destroy(gameObject);
            return;
        }

        // --- 获取或添加 AudioSource 组件 ---
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            // 如果当前游戏对象上没有 AudioSource 组件，就自动添加一个
            Debug.LogWarning("FSXManager: 未找到 AudioSource 组件, 已自动添加。");
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    /// <summary>
    /// 根据名称播放一个音效
    /// </summary>
    /// <param name="name">在 Inspector 列表中定义的音效名称</param>
    public void PlaySound(string name)
    {
        // 使用 Linq 查找具有匹配名称的音效
        SoundEffect s = soundEffects.Find(sound => sound.name == name);

        // 如果没有找到音效，则在控制台打印警告并返回
        if (s == null)
        {
            Debug.LogWarning("音效: " + name + " 未在 FSXManager 中找到！");
            return;
        }

        // 设置音高（如果需要）
        audioSource.pitch = s.pitch;

        // 使用 PlayOneShot 播放音效，这样可以同时播放多个音效而不会互相打断
        // PlayOneShot 的第二个参数是音量缩放系数
        audioSource.PlayOneShot(s.clip, s.volume);
    }
}