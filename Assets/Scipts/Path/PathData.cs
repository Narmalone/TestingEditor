using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AssetData: ", menuName = "New Data Path Asset", order = 1)]
public class PathData : ScriptableObject
{
    List<GameObject> DataObjects;
}
