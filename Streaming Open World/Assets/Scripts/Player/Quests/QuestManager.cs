using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [SerializeField] private List<Quest> quests = new List<Quest>();
    private int zombiesKilled;
    private int pointsCollected;

    public void IncrementZombiesKilled(int kills)
    {
        zombiesKilled += kills;
    }
    
    public int GetZombiesKilled()
    {
        return zombiesKilled;
    }
    
    public void IncrementPointsCollected(int points)
    {
        pointsCollected += points;
    }
    
    public int GetPointsCollected()
    {
        return pointsCollected;
    }
}
