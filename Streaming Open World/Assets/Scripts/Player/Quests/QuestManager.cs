using System;
using System.Collections.Generic;
using Chilli.Quests;
using UnityEngine;

namespace Chilli.Quests
{
    public class QuestManager : MonoBehaviour
    {
        public static QuestManager instance;
        //public GameObject npcPrefab;
        public PrefabCatalogueSO prefabCatalogueSo;
        
        [SerializeField] private List<Quest> quests = new List<Quest>();
        private int zombiesKilled;
        private int pointsCollected;

        private void Awake()
        {
            instance = this;
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
}
