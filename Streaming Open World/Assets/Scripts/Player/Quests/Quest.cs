using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Quest : MonoBehaviour
{
    public enum QuestType
    {
        KillZombies,
        RetrieveItem,
    }
    
    public enum QuestStatus
    {
        Incomplete,
        Complete
    }
    
    [Serializable]
    public class QuestData
    {
        public string name;
        public int rewardPoints;
        public QuestType questType;
        public QuestStatus questStatus;
    }
    
    [SerializeField] private QuestData questData;
    [SerializeField] private GameObject questDialogueText;
    private GameObject player;
    //private bool playerInBounds;

    private void Awake() // cache references
    {
        player = GameObject.FindWithTag("Player").transform.parent.gameObject;
    }

    private void Start()
    {
        questDialogueText.SetActive(false);
    }

    private void Update()
    {
        if (questData.questType == QuestType.KillZombies && questData.questStatus == QuestStatus.Incomplete)
        {
            questDialogueText.GetComponentInChildren<Text>().text = "Kill 5 zombies and I will reward you!";

            if (player.GetComponent<QuestManager>().GetZombiesKilled() >= 5)
            {
                StartCoroutine(PlayQuestCompletionDialogue());
            }
        }
        
        else if (questData.questType == QuestType.RetrieveItem)
        {
            questDialogueText.GetComponentInChildren<Text>().text = "Get me my AXE back and I will reward you well!";
        }

        if (questData.questStatus == QuestStatus.Complete)
        {
            questDialogueText.GetComponentInChildren<Text>().text = "Sorry, I have no quests for you at this time";
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            questDialogueText.SetActive(true);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            questDialogueText.SetActive(false);
        }
    }

    public IEnumerator PlayQuestCompletionDialogue()
    {
        player.GetComponent<QuestManager>().IncrementPointsCollected(questData.rewardPoints);
        questDialogueText.GetComponentInChildren<Text>().text = "Quest Complete! Have yourself " + questData.rewardPoints + "XP";
        questDialogueText.SetActive(true);
        yield return new WaitForSeconds(5.0f);
        questDialogueText.SetActive(false);
        questData.questStatus = QuestStatus.Complete;
    }

    public QuestData GetQuestData()
    {
        return questData;
    }
}
