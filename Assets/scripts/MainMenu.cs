using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // 1. 必须引入场景管理命名空间

public class MainMenu : MonoBehaviour
{
    // 2. 创建一个公开的函数，它接收一个字符串参数（场景名）
    //    这个函数将是所有关卡按钮共用的。
    public void LoadLevel(string sceneName)
    {
        // 3. 检查场景名是否为空，以防万一
        if (!string.IsNullOrEmpty(sceneName))
        {
            Debug.Log("准备加载场景: " + sceneName);
            SceneManager.LoadScene(sceneName); // 4. 加载传入的场景名对应的场景
        }
        else
        {
            Debug.LogError("场景名为空！请在按钮的On Click事件中设置场景名。");
        }
    }

}