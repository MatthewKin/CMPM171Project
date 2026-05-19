using UnityEngine;
using UnityEngine.SceneManagement;

public class NewSceneButton : MonoBehaviour
{
    [Header("Scene To Load")]
    public string sceneName;

    public void LoadScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning("didn't put a scene in breh");
        }
    }
}