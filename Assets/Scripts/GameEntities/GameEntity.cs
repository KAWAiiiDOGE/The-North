using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace TheNorth
{
    public class GameEntity : MonoBehaviour
    {
        [Header("GameEntity Settings")]
        [SerializeField] protected int _maxHealth;
        [SerializeField, Label("Current Health")] protected int _health;
        [SerializeField] protected bool _isInvincible;
        [SerializeField] protected GameEntitySquad _squad;
        [SerializeField] protected bool _isLogTakeDamage = false;

        [HideInInspector] public  UnityEvent<GameEntity, GameEntity, int> Damaged;
        [HideInInspector] public UnityEvent<GameEntity> Died;
        protected GameEntity _target;

        public int MaxHealth => _maxHealth;
        public int Health => _health;
        public bool IsInvincible => _isInvincible;
        public GameEntity Target { get => _target; set => _target = value; }
        public GameEntitySquad Squad { get => _squad; set => _squad = value; }

        public void TakeDamage(GameEntity inflictor, int damage)
        {
            if (_isInvincible || damage <= 0)
                return;

            _health -= damage;
            if(_isLogTakeDamage)
                LogTakeDamage(inflictor, damage);

            Damaged?.Invoke(this, inflictor, damage);

            if (_health <= 0)
            {
                Die();
            }
        }

        public void Die()
        {
            Died?.Invoke(this);

            Damaged.RemoveAllListeners();
            Died.RemoveAllListeners();

            Destroy(gameObject);
        }

        protected void LogTakeDamage(GameEntity inflictor, int damage)
        {
            string message = $"<color=red><b>{inflictor.name}</b>{{{inflictor.GetInstanceID()}}} damaged <b>{gameObject.name}</b>{{{gameObject.GetInstanceID()}}} on <b>{damage} HP</b></color>";
            Debug.Log(message);
        }
    }
}
