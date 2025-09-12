using Scriptables.Utilities;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

namespace Systems.Visual_Effects
{
    public class WindEffect : MonoBehaviour
    {
        [SerializeField] private ObjectPoolDatum windObjectPoolDatum;
        [SerializeField] private ObjectPoolDatum windLoopObjectPoolDatum;
        [SerializeField] private GameObject windPrefab;
        [SerializeField] private GameObject windLoopPrefab;
        
        [SerializeField] private Transform spawnPoint;
        
        [Range(0,1)]
        [SerializeField] private float windLoopPercentage = 0.25f;
        [SerializeField] private Vector3 minSpawnRange;
        [SerializeField] private Vector3 maxSpawnRange;
        [SerializeField] private float spawnRate;
        [SerializeField] private float spawnRateVariation;

        private ObjectPool<ParticleEffect> _windPool;
        private ObjectPool<ParticleEffect> _windLoopPool;
        private float _windTimer;

        private void Awake()
        {
            _windPool = new ObjectPool<ParticleEffect>(windObjectPoolDatum, windPrefab, transform);
            _windLoopPool = new ObjectPool<ParticleEffect>(windLoopObjectPoolDatum, windLoopPrefab, transform);
            _windTimer = spawnRate;
        }

        private void FixedUpdate()
        {
            _windTimer -= Time.fixedDeltaTime;
            if (_windTimer > 0) return;
            SpawnWind();
            _windTimer = spawnRate + Random.Range(-spawnRateVariation, spawnRateVariation);
        }

        private void SpawnWind()
        {
            var randomPos = spawnPoint.position + new Vector3(Random.Range(minSpawnRange.x, maxSpawnRange.x), Random.Range(minSpawnRange.y, maxSpawnRange.y), Random.Range(minSpawnRange.z, maxSpawnRange.z));

            var wind = Random.Range(0f, 1f) > windLoopPercentage ? _windPool.GetObject() : _windLoopPool.GetObject();
            if (!wind) return;
            wind.transform.position = randomPos;
            wind.Finished += ParticleEffectOnFinished;
            wind.Play();
        }

        private void ParticleEffectOnFinished(ParticleEffect particleEffect, int id)
        {
            particleEffect.Finished -= ParticleEffectOnFinished;
            particleEffect.ReturnToPool();
        }
    }
}
