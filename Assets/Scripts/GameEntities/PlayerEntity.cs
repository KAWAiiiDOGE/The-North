using System;
using UnityEngine;
using Zenject;

namespace TheNorth
{
    public class PlayerEntity : Character
    {
        [Header("PlayerEntity Settings")]

        [Inject] readonly InputActions _input;

        void Update()
        {
            if (_canAttack)
            {
                if (_input.Player.Attack.IsPressed())
                {
                    TryAttack(AttackSource);
                }
            }
        }
    }
}
