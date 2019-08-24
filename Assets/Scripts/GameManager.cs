using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager instance = null;

    void Awake()
    {
        // Singleton class
        if(instance != null)
        {
            Destroy(instance);
            return;
        }
        instance = this;
    }
    void Start()
    {

    }
    void Update()
    {   
        
    }
}