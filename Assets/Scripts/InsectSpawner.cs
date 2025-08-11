using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InsectSpawner : MonoBehaviour
{
    [Header("Prefab References")]
    public GameObject mosquitoPrefab;
    public GameObject butterflyPrefab;

    [Header("Pool Settings")]
    public int poolSize = 10;

    // Object Pools
    private Queue<GameObject> mosquitoPool = new Queue<GameObject>();
    private Queue<GameObject> butterflyPool = new Queue<GameObject>();

    // Layer masks 
    [Header("Layers")]
    public LayerMask mosquitoLayer = 8;
    public LayerMask butterflyLayer = 9;

    private bool mustSpawnMosquitoNext = false;

    private void Start()
    {
        InitializePools();
    }

    private void InitializePools()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject mosquito = Instantiate(mosquitoPrefab);
            mosquito.layer = (int)Mathf.Log(mosquitoLayer.value, 2);
            mosquito.SetActive(false);
            mosquitoPool.Enqueue(mosquito);
        }

        for (int i = 0; i < poolSize; i++)
        {
            GameObject butterfly = Instantiate(butterflyPrefab);
            butterfly.layer = (int)Mathf.Log(butterflyLayer.value, 2);
            butterfly.SetActive(false);
            butterflyPool.Enqueue(butterfly);
        }
    }
    public void SpawnInsect(
        int level,
        Vector3 spawnPosition,
        float speed,
        bool walkPausePattern = false,
        float walkTime = 1f,
        float pauseTime = 0.5f)
    {
        InsectType typeToSpawn = DetermineInsectType(level);

        GameObject insectObj = GetPooledInsect(typeToSpawn);
        if (insectObj != null)
        {
            insectObj.transform.position = spawnPosition;
            insectObj.SetActive(true);

            InsectController insect = insectObj.GetComponent<InsectController>();
            insect.Initialize(typeToSpawn, speed, walkPausePattern, walkTime, pauseTime);

            if (typeToSpawn == InsectType.Butterfly)
            {
                float delay = 1.0f;
                StartCoroutine(SpawnMosquitoAfterDelay(spawnPosition, speed, walkPausePattern, walkTime, pauseTime, delay));
            }
        }
    }

    private IEnumerator SpawnMosquitoAfterDelay(
        Vector3 spawnPosition,
        float speed,
        bool walkPausePattern,
        float walkTime,
        float pauseTime,
        float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject mosquitoObj = GetPooledInsect(InsectType.Mosquito);
        if (mosquitoObj != null)
        {
            mosquitoObj.transform.position = spawnPosition;
            mosquitoObj.SetActive(true);

            InsectController insect = mosquitoObj.GetComponent<InsectController>();
            insect.Initialize(InsectType.Mosquito, speed, walkPausePattern, walkTime, pauseTime);
        }
    }

    private InsectType DetermineInsectType(int level)
    {
        if (mustSpawnMosquitoNext)
        {
            mustSpawnMosquitoNext = false;
            return InsectType.Mosquito;
        }

        if (level >= GameManager.Instance.butterflyStartLevel && Random.Range(0f, 1f) < 0.3f)
        {
            return InsectType.Butterfly;
        }

        return InsectType.Mosquito;
    }

    private GameObject GetPooledInsect(InsectType type)
    {
        Queue<GameObject> targetPool = type == InsectType.Mosquito ? mosquitoPool : butterflyPool;

        if (targetPool.Count > 0)
        {
            return targetPool.Dequeue();
        }

        GameObject prefab = type == InsectType.Mosquito ? mosquitoPrefab : butterflyPrefab;
        GameObject newInsect = Instantiate(prefab);
        int layerValue = type == InsectType.Mosquito ?
            (int)Mathf.Log(mosquitoLayer.value, 2) :
            (int)Mathf.Log(butterflyLayer.value, 2);
        newInsect.layer = layerValue;

        return newInsect;
    }

    public void ReturnToPool(GameObject insect, InsectType type)
    {
        insect.SetActive(false);
        Queue<GameObject> targetPool = type == InsectType.Mosquito ? mosquitoPool : butterflyPool;
        targetPool.Enqueue(insect);
    }
}