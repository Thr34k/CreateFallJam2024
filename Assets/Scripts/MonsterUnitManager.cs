using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MonsterUnitManager : MonoBehaviour
{
    public static MonsterUnitManager Instance;

    List<GameObject> activeMonsterUnits = new List<GameObject>();

    [Header("Spawn variables")]
    List<GameObject> monsterSpawns = new List<GameObject>();
    public GameObject nextSpawnToUse;
    public GameObject ghoulPrefab;
    public int maxActiveGhouls = 1;

    private void Awake()
    {
        // Singleton pattern setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        monsterSpawns = GameObject.FindGameObjectsWithTag("MonsterSpawn").ToList();
    }

    private void ChooseNextMonsterSpawn()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (activeMonsterUnits.Count < maxActiveGhouls) 
        {
            SpawnMonster(ghoulPrefab);
        }
    }

    void SpawnMonster(GameObject monsterTypeToSpawn)
    {
        if (nextSpawnToUse != null)
        {
            var viableNextSpawns = monsterSpawns.Where(x => x != nextSpawnToUse).ToList();
            int nextSpawnIndex = UnityEngine.Random.Range(0, viableNextSpawns.Count);
            nextSpawnToUse = monsterSpawns[nextSpawnIndex];
        }
        else 
        { 
            int currentSpawnIndex = UnityEngine.Random.Range(0, monsterSpawns.Count);
            nextSpawnToUse = monsterSpawns[currentSpawnIndex];        
        }

        var spawnedEnemy = Instantiate(monsterTypeToSpawn, nextSpawnToUse.transform.position, ghoulPrefab.transform.rotation);
        spawnedEnemy.GetComponent<Ghoul>().GhouldDied += HandleGhoulDied;
        activeMonsterUnits.Add(spawnedEnemy);
    }

    private void HandleGhoulDied(GameObject deadGhoul)
    {
        activeMonsterUnits.Remove(deadGhoul);
    }
}
