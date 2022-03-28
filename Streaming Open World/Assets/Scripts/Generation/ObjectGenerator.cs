using System;
using System.Collections;
using System.Collections.Generic;
using Chilli.Quests;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectGenerator : MonoBehaviour
{
    public static ObjectGenerator instance;
    public PrefabCatalogueSO prefabCatalogueSo;
    public Material treeMaterial;
    public Mesh[] treeLodMeshes;

    private void Awake()
    {
        instance = this;
    }

    /*[Serializable]
    public class ObjectData
    {
        public string name;
        public Mesh mesh;
        public Mesh collisionMesh;
        public Material[] material;
        [HideInInspector] public Vector3 position;
    }
    
    [SerializeField] private ObjectData[] objectData;

    private GameObject createdObj;*/

    private void Start()
    {
        /*foreach (var obj in objectData)
        {
            GameObject newObj = new GameObject(obj.name);
            newObj.AddComponent<MeshFilter>().sharedMesh = obj.mesh;
            for (int i = 0; i < obj.material.Length; i++)
            {
                newObj.AddComponent<MeshRenderer>().material = obj.material[i];
            }
            newObj.AddComponent<MeshCollider>().sharedMesh = obj.collisionMesh;
            newObj.transform.position = new Vector3(0, 0, 0);
        }*/
        
        //CreateMesh("Tree");
    }

    /*public void CreateMesh(string meshName)
    {
        GameObject newObj = new GameObject(meshName);

        /*foreach (var obj in objectData)
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
        }#1#

        createdObj = newObj;
    }

    public GameObject GetMesh()
    {
        return createdObj;
    }*/
}
