using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;
using static UnityEngine.ParticleSystem;

public class LevelManager : MonoBehaviour
{
    [Header("Enemies Spawning Settings")]

    [Header("Waves")]
    [SerializeField] private int maxWaves = 3;
    [SerializeField] private int enemCountOnFirstWave = 5;
    [SerializeField] private float enemWaveDamageMultiplier = 1f, enemWaveHealthMultiplier = 1f;
    [SerializeField, Tooltip("Multiply enemy count on next waves")] private float enemCountMultiplier = 1.5f;
    [SerializeField] private float baseSpawnDelay = 3f;
    [SerializeField] private bool enableRandomSpawnDelay = true;

    [Header("Random Spawn Positions Settings")]
    [SerializeField] private float playerFreeZoneRadius = 50f;
    [SerializeField] private float maxSpawnDistanceFromPlayer = 200f; 
    [SerializeField, Range(1f, 100f)] private float maxCheckError = 50f;

    [Header("Enemies Settings")]
    [SerializeField] private bool setEnemPatrolZoneIsPlayer = true;
    [SerializeField] private Transform enemPatrolZoneCenter;
    [SerializeField, Range(1f, 100f)] private float enemRandomPointRadius = 50f;
    [SerializeField] private float enemBaseDamageMultiplier = 1f, enemBaseHealthMultiplier = 1f;
    [SerializeField] private GameObject[] enemPrefabs;

    [Space]
    [Header("Player Spawn Settings")]
    [SerializeField] private Transform playerSpawn;
    [SerializeField] private GameObject playerPrefab;

    private GameObject _player;

    private int _currentWave = 0;
    private int _maxEnemCountOnWave;

    private int _enemiesOnLevel = 0;

    private int _bloodChaosCount = 0;
    private int _ichorChaosCount = 0;
    private int _goldChaosCount = 0;

    private void Start()
    {
        EventBus.Instance.SpawnEnemyLayerInd?.AddListener(AddEnemyOnLevel);
        EventBus.Instance.BeingDeadLayerInd?.AddListener(SubtractEnemyOnLevel);
        _maxEnemCountOnWave = enemCountOnFirstWave;

        if (playerSpawn && playerPrefab) SpawnPlayer();

        StartNextWave();
    }

    private void SpawnPlayer()
    {
        if (_player == null)
        {
            _player = Instantiate(playerPrefab, playerSpawn.position, Quaternion.identity);
            if (setEnemPatrolZoneIsPlayer) enemPatrolZoneCenter = _player.transform;
        }
    }

    private void StartNextWave()
    {
        StopAllCoroutines();
        if (_currentWave < maxWaves)
        {
            _currentWave++;

            if (_currentWave > 1)
            {
                _maxEnemCountOnWave = Mathf.RoundToInt(_maxEnemCountOnWave * enemCountMultiplier);
                enemWaveDamageMultiplier *= enemWaveDamageMultiplier;
                enemWaveHealthMultiplier *= enemWaveHealthMultiplier;
            }

            EventBus.Instance.WaveAndEnemiesCountChanged?.Invoke(_currentWave, _maxEnemCountOnWave);
            
            StartCoroutine(EnemySpawner());
        }
    }

    private IEnumerator EnemySpawner()
    {
        int enemPrefabCounter = 0;

        for (int i = 0; i < _maxEnemCountOnWave; i++)
        {
            SpawnEnemyOnRandomPoint(enemPrefabs[enemPrefabCounter]);
            enemPrefabCounter++;

            if (enemPrefabCounter >= enemPrefabs.Length) enemPrefabCounter = 0;

            yield return new WaitForSeconds(GetSpawnTime());
        }
    }

    private void SpawnEnemyOnRandomPoint(GameObject enemy)
    {
        if (enemy)
        {
            for (int i = 0; i <= 10; i++)
            {
                float posX = Random.Range(playerFreeZoneRadius, maxSpawnDistanceFromPlayer) * (Random.value < 0.5f ? -1 : 1);
                float posZ = Random.Range(playerFreeZoneRadius, maxSpawnDistanceFromPlayer) * (Random.value < 0.5f ? -1 : 1);

                Debug.Log("Enemy spawning. Try: " + i);

                if (NavMesh.SamplePosition(new Vector3(posX, maxCheckError / 3, posZ), out NavMeshHit hit, maxCheckError, NavMesh.AllAreas))
                {
                    GameObject spawnedEnemy = Instantiate(enemy, hit.position, Quaternion.identity);
                    SetEnemySettings(ref spawnedEnemy);

                    Debug.Log("Spawned! Position: " + hit.position);

                    return;
                }
            }
            Debug.LogError("Spawn Enemy Random Position NOT FOUND", this);
        }
    }

    private void SetEnemySettings(ref GameObject enemy)
    {
        if (enemy)
        {
            if (enemy.TryGetComponent(out EnemyController ec))
            {
                ec.patrolZoneCenter = enemPatrolZoneCenter;
                ec.randomRadius = enemRandomPointRadius;
            }

            if (enemy.TryGetComponent(out Attacking atc))
            {
                atc.damageMultiplier = enemBaseDamageMultiplier * enemWaveDamageMultiplier;
            }

            if (enemy.TryGetComponent(out Health hp))
            {
                hp.MultiplyMaxHealth(enemBaseHealthMultiplier * enemWaveHealthMultiplier, true);
            }
        }
    }

    private float GetSpawnTime()
    {
        return enableRandomSpawnDelay ? Random.Range(baseSpawnDelay / 2, baseSpawnDelay * 2) : baseSpawnDelay;
    }

    private void AddEnemyOnLevel(int layerInd)
    {
        _enemiesOnLevel++;
        if (layerInd == Constants.BloodChaosLayerIndex) _bloodChaosCount++;
        else if (layerInd == Constants.IchorChaosLayerIndex) _ichorChaosCount++;
        else if (layerInd == Constants.GoldChaosLayerIndex) _goldChaosCount++;
    }

    private void SubtractEnemyOnLevel(string tag, int layerInd)
    {
        if (tag == "Enemy" && _enemiesOnLevel > 0)
        {
            _enemiesOnLevel--;

            if (layerInd == Constants.BloodChaosLayerIndex)
            {
                if (_bloodChaosCount > 0) _bloodChaosCount--;
            }
            else if (layerInd == Constants.IchorChaosLayerIndex)
            {
                if (_ichorChaosCount > 0) _ichorChaosCount--;
            }
            else if (layerInd == Constants.GoldChaosLayerIndex)
            {
                if (_goldChaosCount > 0) _goldChaosCount--;
            }
        }

        if (_enemiesOnLevel <= 0) StartNextWave();
    }

    public GameObject GetActivePlayer()
    {
        return _player;
    }
}
