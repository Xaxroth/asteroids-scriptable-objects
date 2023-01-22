using DefaultNamespace.ScriptableEvents;
using UnityEngine;
using Variables;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Random = UnityEngine.Random;

namespace Asteroids
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Asteroid : MonoBehaviour
    {
        [Header("Asteroid Type")]
        [SerializeField] private BaseAsteroid _asteroidData;

        [Header("Logistics")]
        [SerializeField] private ScriptableEventInt _onAsteroidDestroyed;

        [SerializeField] private SpriteRenderer _asteroidSprite;
        [SerializeField] private Color _asteroidColor;

        [Header("References:")]
        [SerializeField] private Transform _shape;

        private Rigidbody2D _rigidbody;
        private Vector3 _direction;
        private int _instanceId;

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _asteroidSprite = GetComponentInChildren<SpriteRenderer>();
            _instanceId = GetInstanceID();

            LoadAsteroid();
            
            SetDirection();
            AddForce();
            AddTorque();
            SetSize();
        }

        public void LoadAsteroid()
        {
            if (!Directory.Exists(Application.persistentDataPath + "/saved_asteroids/asteroid_data"))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/saved_asteroids/asteroid_data");
            }
            BinaryFormatter bf = new BinaryFormatter();
            if (File.Exists(Application.persistentDataPath + "/saved_asteroids/asteroid_data/asteroid.txt"))
            {
                FileStream file = File.Open(Application.persistentDataPath + "/saved_asteroids/asteroid_data/asteroid.txt", FileMode.Open);
                JsonUtility.FromJsonOverwrite((string)bf.Deserialize(file), _asteroidData);
                _asteroidSprite.sprite = _asteroidData.asteroidSprite;
                _asteroidSprite.color = _asteroidData.asteroidColor;
                file.Close();
            }
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (string.Equals(other.tag, "Laser"))
            {
               HitByLaser();
            }
        }

        private void HitByLaser()
        {
            _onAsteroidDestroyed.Raise(_instanceId);
            Destroy(gameObject);
        }

        // TODO Can we move this to a single listener, something like an AsteroidDestroyer?
        public void OnHitByLaser(IntReference asteroidId)
        {
            if (_instanceId == asteroidId.GetValue())
            {
                Destroy(gameObject);
            }
        }
        
        public void OnHitByLaserInt(int asteroidId)
        {
            if (_instanceId == asteroidId)
            {
                Destroy(gameObject);
            }
        }
        
        private void SetDirection()
        {
            var size = new Vector2(3f, 3f);
            var target = new Vector3
            (
                Random.Range(-_asteroidData._asteroidSize.x, _asteroidData._asteroidSize.x),
                Random.Range(-_asteroidData._asteroidSize.y, _asteroidData._asteroidSize.y)
            );

            _direction = (target - transform.position).normalized;
        }

        private void AddForce()
        {
            var force = Random.Range(_asteroidData._asteroidMinForce, _asteroidData._asteroidMaxForce);
            _rigidbody.AddForce( _direction * force, ForceMode2D.Impulse);
        }

        private void AddTorque()
        {
            var torque = Random.Range(_asteroidData._asteroidMinTorque, _asteroidData._asteroidMaxTorque);
            var roll = Random.Range(0, 2);

            if (roll == 0)
                torque = -torque;
            
            _rigidbody.AddTorque(_asteroidData._asteroidTorque, ForceMode2D.Impulse);
        }

        private void SetSize()
        {
            var size = Random.Range(_asteroidData._asteroidMinSize, _asteroidData._asteroidMaxSize);
            _shape.localScale = new Vector3(size, size, 0f);
        }
    }
}
