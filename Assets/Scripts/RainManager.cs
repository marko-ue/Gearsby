using UnityEngine;

public class RainManager : MonoBehaviour
{
    public bool isRaining = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke("StartRaining", 10f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void StartRaining()
    {
        isRaining = true;
        Invoke("StopRaining", 30f);
    }

    void StopRaining()
    {
        isRaining = false;
    }
}
