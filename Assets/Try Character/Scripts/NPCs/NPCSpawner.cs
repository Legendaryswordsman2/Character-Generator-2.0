using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    [SerializeField] float minSpawnDelay = 8;
    [SerializeField] float maxSpawnDelay = 10;

    [Space]

    [SerializeField] NPCSpawnPoint[] npcSpawnPoints;

    [Space]

    [SerializeField] NPC npcPrefab;

    private void Start()
    {
        for (int i = 0; i < npcSpawnPoints.Length; i++)
        {
            StartCoroutine(SpawnRoutine(npcSpawnPoints[i]));
        }
    }

    IEnumerator SpawnRoutine(NPCSpawnPoint spawnPoint)
    {
        Instantiate(npcPrefab, spawnPoint.SpawnPOS.position, Quaternion.identity, transform).SetEndDestination(spawnPoint.EndPOS);

        yield return new WaitForSeconds(Random.Range(minSpawnDelay, maxSpawnDelay));

        StartCoroutine(SpawnRoutine(spawnPoint));
    }

    [System.Serializable]
    class NPCSpawnPoint
    {
        public Transform SpawnPOS;
        public Transform EndPOS;
    }
}