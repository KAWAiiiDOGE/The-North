using UnityEngine;

namespace TheNorth
{
    [CreateAssetMenu(fileName = "NewWeapon", menuName = "The North/Weapon Data", order = 0)]
    public class WeaponData : ScriptableObject
    {
        public int Damage;
        public float Delay;
        public float Distance;
        public float Speed;
        public LayerMask Layer;

        public GameObject HitEffect; // bullshit
        public AudioClip Swing;
        public AudioClip Hit;
    }
}
