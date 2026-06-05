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
            if(sceneName == "IntroCutscene")
            {
                AudioSource[] allAudio = FindObjectsOfType<AudioSource>();
                foreach (AudioSource a in allAudio)
                {
                    a.Stop();
                }
            }
            SceneManager.LoadScene(sceneName);

        }
        else
        {
            Debug.LogWarning("didn't put a scene in breh");
        }
    }
}