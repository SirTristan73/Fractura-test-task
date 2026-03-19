using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EventBus
{
    public class ParticlePool : MonoBehaviour
    {
        public static ParticlePool Instance { get; private set; }

        [SerializeField] private ParticleSystem _firePrefab;
        [SerializeField] private ParticleSystem _hitPrefab;
        [SerializeField] private int _initialSize = 10;

        private readonly Queue<ParticleSystem> _firePool = new();
        private readonly Queue<ParticleSystem> _hitPool = new();

        private void Awake()
        {
            Instance = this;

            for (int i = 0; i < _initialSize; i++)
            {
                _firePool.Enqueue(CreateParticle(_firePrefab));
                _hitPool.Enqueue(CreateParticle(_hitPrefab));
            }
        }

        private ParticleSystem CreateParticle(ParticleSystem prefab)
        {
            var p = Instantiate(prefab, transform);
            p.gameObject.SetActive(false);
            return p;
        }

        public ParticleSystem GetFire()
        {
            if (_firePool.Count == 0) _firePool.Enqueue(CreateParticle(_firePrefab));
            var p = _firePool.Dequeue();
            p.gameObject.SetActive(true);
            return p;
        }

        public ParticleSystem GetHit()
        {
            if (_hitPool.Count == 0) _hitPool.Enqueue(CreateParticle(_hitPrefab));
            var p = _hitPool.Dequeue();
            p.gameObject.SetActive(true);
            return p;
        }

        public void ReturnDelayed(ParticleSystem p, bool isFire)
        {
            StartCoroutine(ReturnRoutine(p, isFire));
        }

        private IEnumerator ReturnRoutine(ParticleSystem p, bool isFire)
        {
            yield return new WaitWhile(() => p.IsAlive(true));

            p.gameObject.SetActive(false);
            p.transform.SetParent(transform);

            if (isFire) _firePool.Enqueue(p);
            else _hitPool.Enqueue(p);
        }
    }
}