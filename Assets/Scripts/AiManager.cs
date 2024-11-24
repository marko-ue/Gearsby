using UnityEngine;
using System.Collections.Generic;

public class AIManager : MonoBehaviour
{
    private static AIManager instance;
    public static AIManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Object.FindFirstObjectByType<AIManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("AI Manager");
                    instance = go.AddComponent<AIManager>();
                }
            }
            return instance;
        }
    }

    private List<AIController> regularAIs = new List<AIController>();
    private List<StalkerAI> stalkerAIs = new List<StalkerAI>();
    
    private bool playerSpottedByStalker = false;

    public void RegisterRegularAI(AIController ai)
    {
        if (!regularAIs.Contains(ai))
            regularAIs.Add(ai);
    }

    public void RegisterStalkerAI(StalkerAI ai)
    {
        if (!stalkerAIs.Contains(ai))
            stalkerAIs.Add(ai);
    }

    public void UnregisterRegularAI(AIController ai)
    {
        regularAIs.Remove(ai);
    }

    public void UnregisterStalkerAI(StalkerAI ai)
    {
        stalkerAIs.Remove(ai);
    }

    public void OnStalkerSpottedPlayer(Vector3 playerPosition)
    {
        if (!playerSpottedByStalker)
        {
            playerSpottedByStalker = true;
            AlertAllAIs(playerPosition);
        }
    }

    private void AlertAllAIs(Vector3 playerPosition)
    {
        foreach (var ai in regularAIs)
        {
            if (ai != null)
            {
                ai.RespondToAlert(playerPosition, playerPosition, float.MaxValue);
            }
        }
    }

    public void AlertAllNearbyAIs(Vector3 playerPosition, Vector3 alertPosition, float alertRange)
    {
        foreach (var ai in regularAIs)
        {
            if (ai != null)
            {
                ai.RespondToAlert(playerPosition, alertPosition, alertRange);
            }
        }
    }
}