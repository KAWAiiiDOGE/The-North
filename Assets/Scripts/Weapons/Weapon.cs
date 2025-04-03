using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace TheNorth
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private WeaponData _weaponData;

        public static readonly int IdleAnimation = Animator.StringToHash("Idle");
        public static readonly int AttackAnimation = Animator.StringToHash("Attack");

        private static readonly float s_hitEffectClear = 5f;
        private bool _isAllowsAttack = true;
        private WaitForSeconds _waitNextAttack;

        public WeaponData WeaponData => _weaponData;
        public bool IsAllowsAttack => _isAllowsAttack;

        void Start()
        {
            _waitNextAttack = new WaitForSeconds(_weaponData.Delay);
        }

        public bool Hit(Vector3 origin, Vector3 direction, out RaycastHit hitInfo)
        {
            if (!_isAllowsAttack)
            {
                hitInfo = default;
                return false;
            }

            Ray ray = new(origin, direction);
            bool isHit = Physics.Raycast(ray, out RaycastHit raycastHitinfo, _weaponData.Distance);
            if (isHit)
            {
                CreateHitEffect(raycastHitinfo.point, raycastHitinfo.normal);
                // Play Animations
                // Play Sounds
            }
            _isAllowsAttack = false;
            WaitNextAttackCoroutine();
            hitInfo = raycastHitinfo;
            return isHit;
        }

        // TODO: Реализовать совместно с ObjectPool
        void CreateHitEffect(Vector3 position, Vector3 normal)
        {
            GameObject effect = Instantiate(_weaponData.HitEffect, position, Quaternion.LookRotation(normal, Vector3.up));
            Destroy(effect, s_hitEffectClear);
        }

        /// <summary>
        /// Ожидать следующей атаки. Coroutine вариант
        /// </summary>
        void WaitNextAttackCoroutine()
        {
            StartCoroutine(WaitNextAttack());
        }

        IEnumerator WaitNextAttack()
        {
            yield return _waitNextAttack;
            _isAllowsAttack = true;
        }

        // TODO: Заменить на UniTask
        /// <summary>
        /// Ожидать следующей атаки. Async вариант
        /// <para>
        /// Пока не юзабельно т.к. не понял, как внедрить
        /// </para>
        /// </summary>
        async Task WaitNextAttackAsync()
        {
            await Task.Delay(TimeSpan.FromSeconds(_weaponData.Delay / Time.timeScale));
            _isAllowsAttack = true;
        }
    }
}
