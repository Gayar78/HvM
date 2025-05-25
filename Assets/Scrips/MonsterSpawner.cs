using UnityEngine;
using Mirror;
using System.Collections;

public class BotSpawner : NetworkBehaviour
{
    public GameObject botPrefab;
    public Transform[] spawnPoints; // Tu peux assigner des points de spawn dans l'inspector

    public override void OnStartServer()
    {
        base.OnStartServer();
        // Spawn initial
        SpawnBot();
        // Lance la coroutine pour spawner régulièrement
        StartCoroutine(SpawnBotRoutine());
    }

    void SpawnBot()
    {
        // Choisir un point de spawn (aléatoire si plusieurs)
        Vector3 spawnPos = Vector3.zero;
        Quaternion spawnRot = Quaternion.identity;

        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            int idx = Random.Range(0, spawnPoints.Length);
            spawnPos = spawnPoints[idx].position;
            spawnRot = spawnPoints[idx].rotation;
        }

        GameObject bot = Instantiate(botPrefab, spawnPos, spawnRot);
        NetworkServer.Spawn(bot);
    }

    IEnumerator SpawnBotRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(30f); // Attends 30 secondes
            SpawnBot();
        }
    }
}
