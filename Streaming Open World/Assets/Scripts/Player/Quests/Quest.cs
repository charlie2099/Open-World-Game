using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest : MonoBehaviour
{
    [Serializable]
    public class QuestData
    {
        public string name;
        public int progress;
        public bool status;
    }
    
    [SerializeField] private QuestData questData;
}
