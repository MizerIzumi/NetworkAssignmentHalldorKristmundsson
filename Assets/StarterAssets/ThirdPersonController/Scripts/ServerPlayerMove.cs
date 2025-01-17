using Unity.Netcode;
using UnityEngine;
[DefaultExecutionOrder(0)] // Execute before ClientNetworkTransform

public class ServerPlayerMove : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        Debug.Log(ServerPlayerSpawnPoints.Instance.GetRandomSpawnPoint());
        base.OnNetworkDespawn();
        //Only executes on the Server
        if (!IsServer)
        {
            enabled = false;
            return;
        }
        SpawnPlayer();
        base.OnNetworkSpawn();
    }

    // Move to next available position when spawning
    void SpawnPlayer()
    {
        var spawnPoint = ServerPlayerSpawnPoints.Instance.GetRandomSpawnPoint();
        var spawnPosition = spawnPoint ? spawnPoint.transform.position : Vector3.zero;
        transform.position = spawnPosition;
    }
}
