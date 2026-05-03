using UnityEngine;

public class GlobalManager : MonoBehaviour
{
    public static GlobalManager Instance;

    void Awake()
    {
        // If another instance already exists, destroy this one
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Set the singleton instance
        Instance = this;

        // Make it persist between scenes
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}