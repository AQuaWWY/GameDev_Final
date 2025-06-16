// EndPanel.cs

using System;
using UnityEngine;
using UnityEngine.SceneManagement; // 需要这个来进行场景管理 (如重新加载场景)
using UnityEngine.UI;            // 如果你想通过代码控制按钮等UI元素

public class EndPanel : MonoBehaviour
{
    public GameObject endPanelUI; // 在Inspector中拖拽你的结束面板UI对象到这里
    public Text resultText; // 显示结果的文本组件
    public GameObject[] btnlist;

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
        Scene s = SceneManager.GetActiveScene();
        Debug.Log(s.name);
    }

    // 公共方法，由其他脚本调用来触发结束面板
    public void TriggerEndSequence(bool isFailure = false)
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
        if (resultText != null)
        {
            if (isFailure)
            {
                resultText.text = "游戏失败!";
                btnlist[2].SetActive(false);
            }
            else
            {
                resultText.text = "恭喜通关!";
                btnlist[1].SetActive(false);
            }
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // 恢复游戏时间
        // 重新加载当前场景
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f; // 恢复游戏时间
        // 重新加载主场景
        SceneManager.LoadScene("mainMenu");
    }

    public void NextLevel()
    {
        Time.timeScale = 1f; // 恢复游戏时间
        // 重新加载下一关场景
        Scene s = SceneManager.GetActiveScene();
        Debug.Log(s.name);
        if (s.name == "lv1")
        {
            SceneManager.LoadScene("lv2");
        }
        //else if (s.name == "lv2")
        //{
        //    SceneManager.LoadScene("lv3");
        //}
        else
        {
            SceneManager.LoadScene("lv1");
        }
    }
}