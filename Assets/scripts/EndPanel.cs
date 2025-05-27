// EndPanel.cs

using UnityEngine;
using UnityEngine.SceneManagement; // 需要这个来进行场景管理 (如重新加载场景)
using UnityEngine.UI;            // 如果你想通过代码控制按钮等UI元素

public class EndPanel : MonoBehaviour
{
    public GameObject endPanelUI; // 在Inspector中拖拽你的结束面板UI对象到这里
    
    // public Button restartButton;
    // public Button quitButton;

    void Start()
    {
        // 确保游戏开始时结束面板是隐藏的
        if (endPanelUI != null)
        {
            endPanelUI.SetActive(false);
        }
        else
        {
            Debug.LogError("EndPanelUI is not assigned in the EndPanel script!");
        }

        // (可选) 如果有按钮，可以在这里添加监听器
        // if (restartButton != null)
        // {
        //     restartButton.onClick.AddListener(RestartGame);
        // }
        // if (quitButton != null)
        // {
        //     quitButton.onClick.AddListener(QuitGame);
        // }
    }

    // 公共方法，由其他脚本调用来触发结束面板
    public void TriggerEndSequence()
    {
        if (endPanelUI != null)
        {
            endPanelUI.SetActive(true); // 显示结束面板
        }

        Time.timeScale = 0f; // 暂停游戏 (所有基于Time.deltaTime的移动和动画都会停止)
        
        // 当UI显示时，通常需要显示并解锁鼠标光标
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("Game Over! End Panel Triggered.");
    }

    // --- (可选) 以下是给UI按钮调用的方法 ---

    public void RestartGame()
    {
        Time.timeScale = 1f; // 恢复游戏时间
        // 重新加载当前场景
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit(); // 在编辑器中可能不会立即生效，但在构建版本中会退出
    }
}