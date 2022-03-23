using UnityEngine;

public class LOD : MonoBehaviour
{
    public enum LODLevel 
    { 
        LOD0, 
        LOD1,
        LOD2 
    }
    
    public LODLevel currentLOD = LODLevel.LOD0;
    public Mesh[] lodMesh;
    public float distanceLOD1;
    public float distanceLOD2;
    public float updateInterval = 2.0f;

    private MeshFilter meshFilter;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        InvokeRepeating(nameof(CheckLOD), 0, 1.0f);
    }
   
    private void CheckLOD()
    {
        // Distance from this object to the camera
        float distanceFromObject = Vector3.Distance(transform.position, Camera.main.transform.position);

        // If distance from this object to camera is LESS than distance to LOD1 AND current LOD is not 0, set current LOD to 0 and mesh to LOD0 
        if (distanceFromObject < distanceLOD1 && currentLOD != LODLevel.LOD0)
        {
            currentLOD = LODLevel.LOD0;
            meshFilter.mesh = lodMesh[0];
        }
        // If distance from this object to camera is MORE than distance to LOD1 AND LESS than distance to LOD2 AND current LOD is not 1, set current LOD to 1 and mesh to LOD1 
        else if (distanceFromObject >= distanceLOD1 && distanceFromObject < distanceLOD2 && currentLOD != LODLevel.LOD1)
        {
            currentLOD = LODLevel.LOD1;
            meshFilter.mesh = lodMesh[1];
        }
        // If distance from this object to camera is MORE than distance to LOD2 AND current LOD is not 2, set current LOD to 2 and mesh to LOD2 
        else if (distanceFromObject >= distanceLOD2 && currentLOD != LODLevel.LOD2)
        {
            currentLOD = LODLevel.LOD2;
            meshFilter.mesh = lodMesh[2];
        }
    }
    
}
