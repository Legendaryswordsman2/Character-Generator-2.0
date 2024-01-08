using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    public static NPCSpawner Instance;

    [SerializeField] float minSpawnDelay = 8;
    [SerializeField] float maxSpawnDelay = 10;

    [Space]

    [SerializeField] NPCSpawnPoint[] npcSpawnPoints;

    [Space]

    [SerializeField] RuntimeAnimatorController[] npcVariants;

    [Space]

    [SerializeField] NPC npcPrefab;

    private void Awake()
    {
        Instance = this;
    }

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

    public RuntimeAnimatorController GetNPCVariant()
    {
        return npcVariants[Random.Range(0, npcVariants.Length)];
    }

    [System.Serializable]
    class NPCSpawnPoint
    {
        public Transform SpawnPOS;
        public Transform EndPOS;
    }
}