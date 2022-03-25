using System.Collections;
using System.Collections.Generic;
using Chilli.Terrain;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshGenerator : MonoBehaviour
{
    private NavMeshSurface[] surfaces;
    private GameObject terrain;
    
    void Start ()
    {
        StartCoroutine(WaitForTerrainToGenerate());
    }

    private IEnumerator WaitForTerrainToGenerate()
    {
        yield return new WaitForSeconds(1.0f);
        terrain = GameObject.FindWithTag("MainTerrain");

        for (int i = 0; i < terrain.transform.childCount; i++)
        {
            terrain.transform.GetChild(i).AddComponent<NavMeshSurface>();

            if (terrain.transform.GetChild(i).GetComponent<Chunk>().isLoaded)
            {
                yield return new WaitForSeconds(2.5f);
                terrain.transform.GetChild(i).GetComponent<NavMeshSurface>().BuildNavMesh();
            }
        }
    }
}
