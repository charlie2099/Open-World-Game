using UnityEngine;
using UnityEditor;

/// <summary>
/// Batch rename multiple GameObjects in the hierarchy
/// 
/// WARNING:
/// Must sit inside the Editor folder.
/// Anything inside the Editor folder will not be included in the build of the game.
/// Nothing critical to the game should be contained within this script.
/// </summary>

public class BatchRenamerEditor : EditorWindow
{
    private Transform container;
    private string containerName;
    private string newName;

    [MenuItem("My Tools/BatchRenamer")]
    public static void ShowWindow()
    {
        GetWindow(typeof(BatchRenamerEditor));
    }

    private void OnGUI()
    {
        GUILayout.Label("Rename Child GameObjects", EditorStyles.boldLabel);
        
        containerName = EditorGUILayout.TextField("Container Name", containerName);
        newName = EditorGUILayout.TextField("New Name", newName);

        if (GUILayout.Button("Rename GameObjects"))
        {
            BatchRename();
        }
    }

    private void BatchRename()
    {
        // Check container exists
        if (GameObject.Find(containerName) == null)
        {
            Debug.LogError("<color=red> Error: Could not find the container in the scene </color>");
            return;
        }

        container = GameObject.Find(containerName).transform;
        Debug.Log("Child count: " + container.childCount);

        for (int i = 0; i < container.childCount; i++)
        {
            container.GetChild(i).name = newName + " " + i;
        }
        
        Debug.Log("<color=lime> GameObjects successfully renamed! </color>");
    }
}