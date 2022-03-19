using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveNPCData : MonoBehaviour
{
    private class NPCData
    {
        public string name;
        public Mesh mesh;
        public Material material;
        public Vector3 position;
        public ScriptableObject npcData; // i.e. quest data
    }

    [SerializeField] private GameObject npcPrefab;
}
