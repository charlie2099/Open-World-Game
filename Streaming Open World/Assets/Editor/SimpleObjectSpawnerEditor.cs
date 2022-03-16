using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SimpleObjectSpawnerEditor : EditorWindow
{
    static bool _active;
    private GameObject objectToPaint; 

    // Open this from Window menu
    [MenuItem("My Tools/SimpleObjectSpawner")]
    static void Init()
    {
        var window = GetWindow(typeof(SimpleObjectSpawnerEditor));
        window.Show();
    }

    // Listen to scene event
    void OnEnable() => SceneView.duringSceneGui += OnSceneGUI;
    void OnDisable() => SceneView.duringSceneGui -= OnSceneGUI;

    // Receives scene events
    // Use event mouse click for raycasting
    void OnSceneGUI(SceneView view)
    {
        if (!_active)
        {
            return;
        }

        if (Event.current.type == EventType.MouseDown)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RaycastHit hit;

            // Spawn cube on hit location
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("Hit: " + hit.collider.gameObject.name);

                Instantiate(objectToPaint, hit.point, Quaternion.identity);
                // Assign the instantiated object to the chunk that the raycast has hit
            }
        }

        Event.current.Use();
    }

    // Creates a editor window with button 
    // to toggle raycasting on/off
    void OnGUI()
    {
        objectToPaint = EditorGUILayout.ObjectField("Object To Spawn", objectToPaint, typeof(GameObject), false) as GameObject;
        
        if (GUILayout.Button("Enable Raycasting"))
        {
            _active = !_active;
        }

        GUILayout.Label("Active:" + _active);
    }
}
