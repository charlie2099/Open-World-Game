using UnityEngine;
//@AddComponentMenu("Scripts/Misc/LevelOfDetail")
//@RequireComponent(MeshFilter)

/* Script to swap out meshes based on distance (LOD) */
public class LOD : MonoBehaviour
{
    public enum LODLevel 
    { 
        LOD0, 
        LOD1,
        LOD2 
    }
    
    public Mesh lodMesh0;
    public Mesh lodMesh1;
    public Mesh lodMesh2;

    public float distanceLOD1;
    public float distanceLOD2;
    
    public float updateInterval = 2.0f;

    public LODLevel currentLOD = LODLevel.LOD0;

    private MeshFilter meshFilter;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();

        //Distribute load by checkingLOD for every LOD script at a different time
        var startIn = Random.Range(0.0f, updateInterval);
        InvokeRepeating(nameof(CheckLOD), startIn, updateInterval);
    }
   
    private void CheckLOD()
    {
        // Distance from this object to the camera
        float distanceFromObject = Vector3.Distance(transform.position, Camera.main.transform.position);

        // If distance from this object to camera is LESS than distance to LOD1 AND current LOD is not 0, set current LOD to 0 and mesh to LOD0 
        if (distanceFromObject < distanceLOD1 && currentLOD != LODLevel.LOD0)
        {
            currentLOD = LODLevel.LOD0;
            meshFilter.mesh = lodMesh0;
        }
        // If distance from this object to camera is MORE than distance to LOD1 AND LESS than distance to LOD2 AND current LOD is not 1, set current LOD to 1 and mesh to LOD1 
        else if (distanceFromObject >= distanceLOD1 && distanceFromObject < distanceLOD2 && currentLOD != LODLevel.LOD1)
        {
            currentLOD = LODLevel.LOD1;
            meshFilter.mesh = lodMesh1;
        }
        // If distance from this object to camera is MORE than distance to LOD2 AND current LOD is not 2, set current LOD to 2 and mesh to LOD2 
        else if (distanceFromObject >= distanceLOD2 && currentLOD != LODLevel.LOD2)
        {
            currentLOD = LODLevel.LOD2;
            meshFilter.mesh = lodMesh2;
        }
    }
    
}


/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines the level of detail (LOD) of a model based on it's distance to the camera
/// </summary>

public class LOD : MonoBehaviour
{
    // LOD 0: Normal level of detail 
    // LOD 1: Less detailed model (fewer triangles)
    // LOD 2: billboard
}*/
