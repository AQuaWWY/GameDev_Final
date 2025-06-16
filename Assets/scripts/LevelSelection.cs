using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelection : MonoBehaviour
{
    public string targetSceneName;

    public void LoadLevel()
    {
        SceneManager.LoadScene(targetSceneName);
    }
}