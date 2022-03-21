using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class SimpleObjectSpawnerEditor : EditorWindow
{
    private static bool _activeAssigner;
    private static bool _activeDeassigner;
    private GameObject objectToPaint; 
    
    [MenuItem("My Tools/SimpleObjectSpawner")]
    private static void Init()
    {
        var window = GetWindow(typeof(SimpleObjectSpawnerEditor));
        window.Show();
    }

    private void OnEnable() => SceneView.duringSceneGui += OnSceneGUI;
    private void OnDisable() => SceneView.duringSceneGui -= OnSceneGUI;

    private void OnSceneGUI(SceneView view)
    {
        /*if (!_activeAssigner)
        {
            return;
        }*/

        // if activeAssigner is true, perform the following code

        if (Event.current.type == EventType.MouseDown)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RaycastHit hit;
        
            if (Physics.Raycast(ray, out hit))
            {
                //Debug.Log("Hit: " + hit.collider.gameObject.name);

                if (_activeAssigner)
                {
                    GameObject spawnedObj = Instantiate(objectToPaint, hit.point, Quaternion.identity);
                    hit.collider.gameObject.GetComponent<Chunk>().chunkObjects.Add(spawnedObj);
                    spawnedObj.transform.parent = hit.transform;
                }

                if (_activeDeassigner)
                {
                    var chunkObj = hit.collider.gameObject.GetComponent<Chunk>().chunkObjects;
                    if (chunkObj != null)
                    {
                        foreach (var obj in chunkObj.ToArray())
                        {
                            DestroyImmediate(obj, true);
                            chunkObj.Clear();
                        }
                    }
                }
            }
            Event.current.Use();
        }
    }

    private void OnGUI()
    {
        GUILayout.Space(10);
        objectToPaint = EditorGUILayout.ObjectField("Object To Spawn", objectToPaint, typeof(GameObject), false) as GameObject;
        
        GUILayout.Space(10);
        if (GUILayout.Button("Enable Spawner"))
        {
            _activeAssigner = !_activeAssigner;
        }
        GUILayout.Label("Spawner Mode:" + _activeAssigner);
        
        GUILayout.Space(10);
        if (GUILayout.Button("Enable De-assigner"))
        {
            _activeDeassigner = !_activeDeassigner;
        }
        GUILayout.Label("DeAssigner Mode:" + _activeDeassigner);
    }
}
