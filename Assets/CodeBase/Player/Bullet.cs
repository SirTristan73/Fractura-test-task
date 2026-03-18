using UnityEngine;

namespace EventBus
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float _speed = 20f;
        [SerializeField] private float _lifetime = 3f;
        [SerializeField] private ParticleSystem _hitEffect;

        private float _damage;
        private float _lifetimeTimer;

        public void Init(Vector3 position, Quaternion rotation, float damage)
        {
            transform.SetParent(null);
            transform.position = position;
            transform.rotation = rotation;
            _damage = damage;
            _lifetimeTimer = _lifetime;
        }

        private void Update()
        {
            transform.position += transform.forward * _speed * Time.deltaTime;

            _lifetimeTimer -= Time.deltaTime;
            if (_lifetimeTimer <= 0f)
                BulletPool.Instance.Return(this);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out IDamageable damageable)) return;
            Debug.Log(damageable);
            damageable.TakeDamage(_damage);

            if (_hitEffect != null)
            {
                _hitEffect.transform.SetParent(null);
                _hitEffect.Play();
            }

            BulletPool.Instance.Return(this);
        }
    }
}