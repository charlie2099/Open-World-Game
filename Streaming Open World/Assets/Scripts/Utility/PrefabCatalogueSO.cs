using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PrefabCatalogueData", menuName = "My Scriptable Objects/PrefabCatalogue")]
public class PrefabCatalogueSO : ScriptableObject
{
    public List<GameObject> prefab;
    //public int id;
    //private Dictionary<int, GameObject> dictionary;
}
