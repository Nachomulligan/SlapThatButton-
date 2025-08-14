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
        // Crear pool de mosquitos
        for (int i = 0; i < poolSize; i++)
        {
            GameObject mosquito = Instantiate(mosquitoPrefab);
            mosquito.layer = (int)Mathf.Log(mosquitoLayer.value, 2);
            mosquito.SetActive(false);
            mosquitoPool.Enqueue(mosquito);
        }

        // Crear pool de mariposas
        for (int i = 0; i < poolSize; i++)
        {
            GameObject butterfly = Instantiate(butterflyPrefab);
            butterfly.layer = (int)Mathf.Log(butterflyLayer.value, 2);
            butterfly.SetActive(false);
            butterflyPool.Enqueue(butterfly);
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

            // Determinar tipo de movimiento basado en nivel y probabilidad
            IMovementStrategy movementStrategy = DetermineMovementStrategy(level, speed);

            InsectController insect = insectObj.GetComponent<InsectController>();
            insect.Initialize(typeToSpawn, movementStrategy);

            // Spawn mosquito después de butterfly (lógica original)
            if (typeToSpawn == InsectType.Butterfly)
            {
                float delay = 1.0f;
                StartCoroutine(SpawnMosquitoAfterDelay(spawnPosition, speed, delay));
            }
        }
    }

    private IMovementStrategy DetermineMovementStrategy(int level, float speed)
    {
        // A partir del nivel 2, introducir movimiento con pausas
        if (level >= 2 && Random.Range(0f, 1f) < walkPauseChance)
        {
            return new WalkPauseMovement(speed, walkDuration, pauseDuration);
        }

        // Más adelante puedes agregar más estrategias basadas en nivel
        // if (level >= 5 && Random.Range(0f, 1f) < 0.2f)
        // {
        //     return new ZigzagMovement(speed, 2f, 2f);
        // }

        return new StraightMovement(speed);
    }

    private IEnumerator SpawnMosquitoAfterDelay(Vector3 spawnPosition, float speed, float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject mosquitoObj = GetPooledInsect(InsectType.Mosquito);
        if (mosquitoObj != null)
        {
            mosquitoObj.transform.position = spawnPosition;
            mosquitoObj.SetActive(true);

            // El mosquito que aparece después de la mariposa siempre va recto
            IMovementStrategy straightStrategy = new StraightMovement(speed);

            InsectController insect = mosquitoObj.GetComponent<InsectController>();
            insect.Initialize(InsectType.Mosquito, straightStrategy);
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

        // Si no hay objetos en el pool, crear uno nuevo
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