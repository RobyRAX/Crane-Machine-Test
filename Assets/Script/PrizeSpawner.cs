using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrizeSpawner : MonoBehaviour
{
    public static PrizeSpawner Instance;

    public static event Action OnPrizeSpawnDone;
    
    [Header("Spawn Area Parameter")]
    [SerializeField] Vector3 spawnArea;
    [SerializeField] Vector3 spawnCenter;
    [SerializeField] float divider;
    public List<Vector3> spawnPoints;

    [Header("Prizes Parameter")]   
    [SerializeField] int prizeCount;
    [SerializeField] float spawnDuration;
    public Transform prizeParent;
    public GameObject[] prizePrefabs;
    public GameObject respawnCol;

    private void Awake()
    {
        Instance = this;

        GameManager.OnGameStateChanged += GameStateChangeHandler;
    }

    private void Start()
    {
        
    }

    void GameStateChangeHandler(GameState state)
    {
        if(state == GameState.GameInit)
        {
            SetSpawnPoints();
            StartCoroutine(SpawnPrizes());
        }
    }

    void SetSpawnPoints()
    {
        for (float i = 0; i <= divider; i++)
        {
            float x = -spawnArea.x + ((spawnArea.x * 2) * (i / divider));

            for (float j = 0; j <= divider; j++)
            {
                float z = -spawnArea.z + ((spawnArea.z * 2) * (j / divider));
                /*Instantiate(test, new Vector3(x, 0, z) + spawnCenter, Quaternion.identity);*/
                spawnPoints.Add(new Vector3(x, 0, z) + spawnCenter);
            }
        }
    }

    IEnumerator SpawnPrizes()
    {
        for(int i = 0; i < prizeCount; i++)
        {
            int random = UnityEngine.Random.Range(0, prizePrefabs.Length);
            GameObject prize = Instantiate(prizePrefabs[random], prizeParent);
            prize.name = prizePrefabs[random].name;
            prize.transform.position = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Count)];
            prize.transform.rotation = Quaternion.Euler(UnityEngine.Random.Range(-360, 360), UnityEngine.Random.Range(-360, 360), UnityEngine.Random.Range(-360, 360));

            yield return new WaitForSeconds(spawnDuration / prizeCount);
        }
        OnPrizeSpawnDone();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(spawnCenter, Vector3.one);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(spawnCenter + spawnArea, spawnCenter + new Vector3(-spawnArea.x, spawnArea.y, spawnArea.z));
        Gizmos.DrawLine(spawnCenter + new Vector3(spawnArea.x, spawnArea.y, -spawnArea.z), spawnCenter + new Vector3(-spawnArea.x, spawnArea.y, -spawnArea.z));

        Gizmos.color = Color.red;
        Gizmos.DrawLine(spawnCenter + spawnArea, spawnCenter + new Vector3(spawnArea.x, spawnArea.y, -spawnArea.z));
        Gizmos.DrawLine(spawnCenter + new Vector3(-spawnArea.x, spawnArea.y, spawnArea.z), spawnCenter + new Vector3(-spawnArea.x, spawnArea.y, -spawnArea.z));

        for (float i = 0; i <= divider; i++)
        {
            //float y = spawnArea.y + spawnCenter.y;
            float x = -spawnArea.x + ((spawnArea.x * 2) * (i / divider));      

            for (float j = 0; j <= divider; j++)
            {
                float z = -spawnArea.z + ((spawnArea.z * 2) * (j / divider));
                Gizmos.DrawSphere(new Vector3(x, 0, z) + spawnCenter, 0.125f);
            }

        }
    }
}
