using System;
using NaughtyAttributes;
using UnityEngine;

namespace TheNorth
{
    public class Character : GameEntity
    {
        // TODO: Weapon realization
        [Header("Character Settings")]
        [SerializeField] protected bool _canAttack = true;
        [SerializeField, ShowIf(nameof(_canAttack))] protected Transform _attackSource;
        [SerializeField, ShowIf(nameof(_canAttack))] protected Weapon _weapon;
        [SerializeField, ShowIf(nameof(_canAttack))] protected Animator _animator;
        [SerializeField, ShowIf(nameof(_canAttack))] protected bool _isLogTryAttack = false;

        public bool CanAttack => _canAttack;
        public bool HasWeapon => _weapon;
        public Transform AttackSource => _attackSource;
        public Weapon Weapon => _weapon;
        public Animator Animator => _animator;

        protected void OnDrawGizmosSelected()
        {
            if (!_canAttack)
                return;

            var ray = new Ray(_attackSource.position, _attackSource.forward);
            bool isHit = Physics.Raycast(ray, out var hitInfo, _weapon.WeaponData.Distance);

            Gizmos.color = Color.red;
            Gizmos.DrawRay(_attackSource.position, _attackSource.forward * _weapon.WeaponData.Distance);

            if (isHit)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(hitInfo.point, Math.Clamp(1f / hitInfo.distance, 0.1f, 0.2f));
            }
        }

        public virtual bool TryAttack(Transform attackSource)
        {
            if (_weapon == null)
                return false;
            if (!_weapon.IsAllowsAttack)
                return false;

            // TODO: Use animator
            bool isHit = _weapon.Hit(attackSource.position, attackSource.forward, out RaycastHit hitInfo);
            if (isHit)
            {
                bool isHittedGameEntity = hitInfo.collider.TryGetComponent(out GameEntity hittedEntity);
                if (isHittedGameEntity)
                    hittedEntity.TakeDamage(this, _weapon.WeaponData.Damage);

                if (_isLogTryAttack)
                    LogTryAttack(hitInfo.collider.gameObject);
            }

            return true;
        }

        protected virtual void LogTryAttack(UnityEngine.Object hittedObject)
        {
            string message = $"<b>{gameObject.name}</b>{{{gameObject.GetInstanceID()}}} attacked <b>{hittedObject.name}</b>{{{hittedObject.GetInstanceID()}}}";
            Debug.Log(message);
        }
    }
}
