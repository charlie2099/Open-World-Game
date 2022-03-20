using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectGenerator : MonoBehaviour
{
    [Serializable]
    public class ObjectData
    {
        public string name;
        public Mesh mesh;
        public Material material;
        [HideInInspector] public Vector3 position;
    }
    
    [SerializeField] private ObjectData objectData;

    private void Start()
    {
        GameObject gameObject = new GameObject(objectData.name);
        gameObject.AddComponent<MeshFilter>().sharedMesh = objectData.mesh;
        gameObject.AddComponent<MeshCollider>().sharedMesh = objectData.mesh;
        gameObject.AddComponent<MeshRenderer>().material = objectData.material;
        gameObject.transform.position = new Vector3(0, 0, 0);
    }
}
