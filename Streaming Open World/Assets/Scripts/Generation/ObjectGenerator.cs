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
        public Mesh collisionMesh;
        public Material[] material;
        [HideInInspector] public Vector3 position;
    }
    
    [SerializeField] private ObjectData[] objectData;

    private GameObject createdObj;

    private void Start()
    {
        foreach (var obj in objectData)
        {
            GameObject newObj = new GameObject(obj.name);
            newObj.AddComponent<MeshFilter>().sharedMesh = obj.mesh;
            for (int i = 0; i < obj.material.Length; i++)
            {
                newObj.AddComponent<MeshRenderer>().material = obj.material[i];
            }
            newObj.AddComponent<MeshCollider>().sharedMesh = obj.collisionMesh;
            newObj.transform.position = new Vector3(0, 0, 0);
        }
    }

    public void CreateMesh(string name)
    {
        GameObject newObj = new GameObject(name);

        foreach (var obj in objectData)
        {
            if (obj.name == name)
            {
                newObj.AddComponent<MeshFilter>().sharedMesh = obj.mesh;
                for (int i = 0; i < obj.material.Length; i++)
                {
                    newObj.AddComponent<MeshRenderer>().material = obj.material[i];
                }
                newObj.AddComponent<MeshCollider>().sharedMesh = obj.collisionMesh;
                
                
            }
        }

        createdObj = newObj;
    }

    public GameObject GetMesh()
    {
        return createdObj;
    }
}
