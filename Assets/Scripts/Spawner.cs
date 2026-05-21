using UnityEngine;

public class Spawner : MonoBehaviour
{
    [System.Serializable]
    public struct SpawnableObject
    {
        public GameObject prefab;
        [Range(0f, 1f)]
        public float spawnChance;
    }

    public SpawnableObject[] objects;

    public float minSpawnRate = 1f;
    public float maxSpawnRate = 2f;

    [Header("Street Lamps")]
    public GameObject lampPrefab;
    public float lampSpawnInterval = 10f; // fixed even spacing

    private void OnEnable()
    {
        Invoke(nameof(Spawn), Random.Range(minSpawnRate, maxSpawnRate));
        Invoke(nameof(SpawnLamp), lampSpawnInterval);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    private void Spawn()
    {
        float spawnChance = Random.value;

        foreach (var obj in objects)
        {
            if (spawnChance < obj.spawnChance)
            {
                GameObject obstacle = Instantiate(obj.prefab);
                obstacle.transform.position += transform.position;
                break;
            }

            spawnChance -= obj.spawnChance;
        }

        Invoke(nameof(Spawn), Random.Range(minSpawnRate, maxSpawnRate));
    }

    private void SpawnLamp()
    {
        if (lampPrefab != null)
        {
            GameObject lamp = Instantiate(lampPrefab);
            lamp.transform.position = transform.position;
        }

        Invoke(nameof(SpawnLamp), lampSpawnInterval);
    }
}