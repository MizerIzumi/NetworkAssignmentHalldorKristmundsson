using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class  ServerPlayerSpawnPoints : MonoBehaviour
{
    public static ServerPlayerSpawnPoints Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

        [SerializeField] private List<GameObject> m_SpawnPoints;
    public GameObject GetRandomSpawnPoint()
    {
        if (m_SpawnPoints.Count == 0) return null;

        return m_SpawnPoints[Random.Range(0, m_SpawnPoints.Count)];
    }
}
