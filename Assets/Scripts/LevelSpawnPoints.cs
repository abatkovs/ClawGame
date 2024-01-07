using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "LevelSpawnPoints", menuName = "Data/LevelSpawnPoints", order = 1)]
public class LevelSpawnPoints : ScriptableObject
{

    [SerializeField] private List<Vector3> SpawnPoints = new List<Vector3>();

    public Vector3 GetRandomSpawnPoint()
    {
        if (SpawnPoints.Count == 0)
        {
            Debug.LogError("No spawn points found");
            return Vector3.zero;
        }
        return SpawnPoints[Random.Range(0, SpawnPoints.Count)];
    }
}