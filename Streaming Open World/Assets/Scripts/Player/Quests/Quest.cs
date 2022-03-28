using System;
using System.Collections;
using Chilli.Quests;
using StarterAssets;
using UnityEngine;
using UnityEngine.UI;

namespace Chilli.Quests
{
    public class Quest : MonoBehaviour
    {
        public enum QuestType
        {
            KillZombiesI,
            KillZombiesII,
            KillZombiesIII,
            KillZombiesIV,
            KillZombiesV,
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
        private GameObject questDialogueText;
        private GameObject player;
        private int zombiesKilled;
        private int zombiesToKill;
        private bool questCollected;
        private StarterAssetsInputs starterAssetsInputs;

        private void OnEnable()
        {
            EventManager.StartListening("ZombieKilled", IncrementKills);
        }
        
        private void OnDisable()
        {
            EventManager.StopListening("ZombieKilled", IncrementKills);
        }

        private void OnApplicationQuit()
        {
            EventManager.StopListening("ZombieKilled", IncrementKills);
        }

        private void IncrementKills(EventParam eventParam)
        {
            if (questCollected)
            {
                zombiesKilled += 1;
                zombiesToKill -= 1;
            }
        }

        private void Awake() // cache references
        {
            questDialogueText = GetComponentInChildren<Image>().gameObject;
            player = GameObject.FindWithTag("Player").transform.parent.gameObject;
        }

        private void Start()
        {
            switch (questData.questType)
            {
                case QuestType.KillZombiesI:
                    zombiesToKill = 5;
                    break;
                case QuestType.KillZombiesII:
                    zombiesToKill = 10;
                    break;
                case QuestType.KillZombiesIII:
                    zombiesToKill = 15;
                    break;
                case QuestType.KillZombiesIV:
                    zombiesToKill = 20;
                    break;
                case QuestType.KillZombiesV:
                    zombiesToKill = 25;
                    break;
            }
            
            questDialogueText.SetActive(false);
            starterAssetsInputs = player.GetComponentInChildren<StarterAssetsInputs>();
        }

        private void Update()
        {
            if (!questCollected)
            {
                if(starterAssetsInputs.interact)
                {
                    questCollected = true;
                }
                else
                {
                    questDialogueText.GetComponentInChildren<Text>().text = "I have a quest for you! (Press E to collect)";
                    return;
                }
            }
            
            
            foreach(QuestType questType in Enum.GetValues(typeof(QuestType)))
            {
                if (questData.questType == questType && questData.questStatus == QuestStatus.Incomplete)
                {
                    questDialogueText.GetComponentInChildren<Text>().text = "[Zombie Quest IV] Kill " + zombiesToKill + " zombies and I will reward you!";
                    
                    if (zombiesKilled >= 20)            
                    {
                        StartCoroutine(PlayQuestCompletionDialogue());
                    }
                }
            }

            if (questData.questType == QuestType.RetrieveItem)
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
}
