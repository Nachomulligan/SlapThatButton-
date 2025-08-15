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

    [Header("Movement Settings")]
    public MovementType defaultMovementType = MovementType.Straight;
    [Range(0f, 1f)] public float walkPauseChance = 0.3f;
    public float walkDuration = 1f;
    public float pauseDuration = 0.5f;
    public float zigzagAmplitude = 2f;
    public float zigzagFrequency = 2f;
    public float randomMoveInterval = 1f;

    [Header("Layers")]
    public LayerMask mosquitoLayer = 8;
    public LayerMask butterflyLayer = 9;

    private Queue<GameObject> mosquitoPool = new Queue<GameObject>();
    private Queue<GameObject> butterflyPool = new Queue<GameObject>();

    //Para cancelar spawns de mosquito asociados a mariposas
    private Dictionary<GameObject, Coroutine> butterflySpawnRoutines = new Dictionary<GameObject, Coroutine>();

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

    public void StartRound(int level, float speed, float spawnInterval, int totalInsects)
    {
        StartCoroutine(StartRoundWithDelay(level, speed, spawnInterval, totalInsects));
    }

    private IEnumerator StartRoundWithDelay(int level, float speed, float spawnInterval, int totalInsects)
    {
        yield return new WaitForSeconds(2f);

        for (int i = 0; i < totalInsects; i++)
        {
            Vector3 spawnPosition = new Vector3(10f, Random.Range(-3f, 3f), 0f);
            SpawnInsect(level, spawnPosition, speed);
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    public void SpawnInsect(int level, Vector3 spawnPosition, float speed)
    {
        InsectType typeToSpawn = DetermineInsectType(level);
        GameObject insectObj = GetPooledInsect(typeToSpawn);

        if (insectObj != null)
        {
            insectObj.transform.position = spawnPosition;
            insectObj.SetActive(true);

            IMovementStrategy movementStrategy = DetermineMovementStrategy(level, speed);
            InsectController insect = insectObj.GetComponent<InsectController>();
            insect.Initialize(typeToSpawn, movementStrategy);

            if (typeToSpawn == InsectType.Butterfly)
            {
                float delay = 1.0f;
                Coroutine routine = StartCoroutine(SpawnMosquitoAfterDelay(insectObj, spawnPosition, speed, delay));
                butterflySpawnRoutines[insectObj] = routine;
            }
        }
    }

    private IMovementStrategy DetermineMovementStrategy(int level, float speed)
    {
        float r = Random.value;

        if (level < 2)
            return new StraightMovement(speed);

        if (level < 5)
        {
            if (r < 0.3f) return new WalkPauseMovement(speed, walkDuration, pauseDuration);
            if (r < 0.6f) return new ZigzagMovement(speed, zigzagAmplitude, zigzagFrequency);
            return new StraightMovement(speed);
        }

        if (level < 10)
        {
            if (r < 0.25f) return new WalkPauseMovement(speed, walkDuration, pauseDuration);
            if (r < 0.5f) return new ZigzagMovement(speed, zigzagAmplitude, zigzagFrequency);
            if (r < 0.75f) return new RandomMovement(speed, randomMoveInterval);
            return new StraightMovement(speed);
        }

        if (r < 0.25f) return new RandomMovement(speed, randomMoveInterval);
        if (r < 0.5f) return new ZigzagMovement(speed, zigzagAmplitude, zigzagFrequency);
        if (r < 0.75f) return new WalkPauseMovement(speed, walkDuration, pauseDuration);
        return new StraightMovement(speed + Random.Range(-0.5f, 0.5f));
    }

    private IEnumerator SpawnMosquitoAfterDelay(GameObject butterfly, Vector3 spawnPosition, float speed, float delay)
    {
        yield return new WaitForSeconds(delay);

        // Solo spawnear mosquito si la mariposa sigue activa
        if (butterfly != null && butterfly.activeInHierarchy)
        {
            GameObject mosquitoObj = GetPooledInsect(InsectType.Mosquito);
            if (mosquitoObj != null)
            {
                mosquitoObj.transform.position = spawnPosition;
                mosquitoObj.SetActive(true);

                IMovementStrategy straightStrategy = new StraightMovement(speed);
                InsectController insect = mosquitoObj.GetComponent<InsectController>();
                insect.Initialize(InsectType.Mosquito, straightStrategy);
            }
        }

        if (butterflySpawnRoutines.ContainsKey(butterfly))
        {
            butterflySpawnRoutines.Remove(butterfly);
        }
    }

    public void CancelButterflyMosquitoSpawn(GameObject butterfly)
    {
        if (butterflySpawnRoutines.TryGetValue(butterfly, out Coroutine routine))
        {
            StopCoroutine(routine);
            butterflySpawnRoutines.Remove(butterfly);
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
        int layerValue = type == InsectType.Mosquito ?
            (int)Mathf.Log(mosquitoLayer.value, 2) :
            (int)Mathf.Log(butterflyLayer.value, 2);

        GameObject newInsect = Instantiate(prefab);
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
