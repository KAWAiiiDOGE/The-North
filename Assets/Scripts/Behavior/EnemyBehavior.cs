using System.Collections.Generic;
using NaughtyAttributes;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

namespace TheNorth
{
    /// <para>
    /// Идея зависимостей компонентов примерно такова и больше никакова:
    /// <see cref="BehaviorGraphAgent">BehaviorGraphAgent</see>
    /// Управляет поведением сущности использую свойства и методы NPC
    /// BehaviorGraphAgent { 
    ///                     - Character {derived from NPC}
    ///                     + BehaviorMethods()
    ///                     + NPCBehaviorMethods()
    ///                     }
    /// 
    /// <see cref="EnemyBehavior">EnemyBehavior</see>
    /// Инициализирует и визуализирует поведение
    /// EnemyBehavior { 
    ///                 - BehaviorGraphAgent
    ///                 - NPC
    ///                 + InitBehavior()
    ///                 + VisualizeBehavior()
    ///                 }
    /// 
    /// <see cref="NPC">NPC</see>
    /// Хранит свойства и задаёт базовые методы поведения сущности
    /// NPC {
    ///         + Properties
    ///         + Methods()
    ///     }
    /// </para>

    /// <summary>
    /// Класс для инициализации и визуализации BehaviorGraphAgent
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent), typeof(BehaviorGraphAgent))]
    public class EnemyBehavior : MonoBehaviour
    {
        [Header("Behavior Settings")]
        [SerializeField] GameObject _waypointPrefab;    // TODO: Zenject [Inject]
        [SerializeField] List<Transform> _waypoints;

        [SerializeField, Foldout("Dynamic Fields"), ReadOnly] List<Vector3> _currentWaypoints;
        [SerializeField, Foldout("Dynamic Fields"), ReadOnly] Vector3 _currentTargetPosition;
        [SerializeField, Foldout("Dynamic Fields"), ReadOnly] GameObject _currentTargetGameObject;

        [Button("Add Waypoint")] void AddWaypoint_Button() => AddWaypoint(transform.position + transform.forward + transform.up);
        [Button("Clear Waypoints")] void ClearWaypoints_Button() => ClearWaypoints();

        BehaviorGraphAgent _behaviorGraphAgent;
        Character _character;
        GameObject _parentWaypoints;
        
        // TODO: Сделать в отдельном классе парсер всех BlackboardVariable
        BlackboardVariable<List<Vector3>> _waypointsBBV;
        BlackboardVariable<Character> _enemyCharacterBBV;
        BlackboardVariable<Transform> _attackSourceBBV;
        BlackboardVariable<Vector3> _targetPositionBBV;
        BlackboardVariable<GameObject> _targetGameObjectBBV;

        public bool HasWaypoints => _waypoints.Count != 0;
        public List<Transform> Waypoints { get => _waypoints; set => _waypoints = value; }

        void Awake()
        {
            _character = _character != null ? _character : GetComponent<Character>();

            _behaviorGraphAgent = _behaviorGraphAgent != null ? _behaviorGraphAgent : GetComponent<BehaviorGraphAgent>();
            _behaviorGraphAgent.Init();

            _behaviorGraphAgent.BlackboardReference.GetVariable("EnemyCharacter", out _enemyCharacterBBV);
            _enemyCharacterBBV.Value = _enemyCharacterBBV.Value != null ? _enemyCharacterBBV.Value : _character;
            
            _behaviorGraphAgent.BlackboardReference.GetVariable("Attack Source", out _attackSourceBBV);
            _attackSourceBBV.Value = _attackSourceBBV.Value != null ? _attackSourceBBV.Value : _character.AttackSource;

            _behaviorGraphAgent.BlackboardReference.GetVariable("Waypoints", out _waypointsBBV);

            _behaviorGraphAgent.BlackboardReference.GetVariable("TargetPosition", out _targetPositionBBV);
            
            _behaviorGraphAgent.BlackboardReference.GetVariable("TargetGameObject", out _targetGameObjectBBV);
        }
        void OnEnable()
        {
            _waypointsBBV.OnValueChanged += OnWaypointsValueChanged;
            _targetPositionBBV.OnValueChanged += OnTargetPositionValueChanged;
            _targetGameObjectBBV.OnValueChanged += OnTargetGameObjectValueChanged;
        }
        void OnDisable()
        {
            _waypointsBBV.OnValueChanged -= OnWaypointsValueChanged;
            _targetPositionBBV.OnValueChanged -= OnTargetPositionValueChanged;
            _targetGameObjectBBV.OnValueChanged -= OnTargetGameObjectValueChanged;
        }
        void OnDrawGizmosSelected()
        {
            DrawWaypoints();
        }

        /// <summary>
        /// Creates waypoint in object's position
        /// </summary>
        public void AddWaypoint()
        {
            AddWaypoint(transform.position + transform.up);
        }
        /// <summary>
        /// Creates waypoint in position
        /// </summary>
        public void AddWaypoint(Vector3 position)
        {
            if (!_parentWaypoints)
            {
                _parentWaypoints = new GameObject($"Waypoints ({name} [{GetInstanceID()}])");
                _parentWaypoints.transform.position = position;
            }
            
            GameObject waypoint = Instantiate(_waypointPrefab, position, Quaternion.identity, _parentWaypoints.transform);
            waypoint.name = $"Waypoint {_waypoints.Count}";
            
            _waypoints.Add(waypoint.transform);
        }
        /// <summary>
        /// Destroys GameObjects and clears List of waypoints
        /// </summary>
        public void ClearWaypoints()
        {
            foreach (var waypoint in _waypoints)
            {
                if (waypoint != null)
                    DestroyImmediate(waypoint.gameObject);
            }
            _waypoints.Clear();

            DestroyImmediate(_parentWaypoints);
            _parentWaypoints = null;
        }

        void DrawWaypoints()
        {
            if (_currentWaypoints != null && _currentWaypoints.Count != 0)
                foreach (Vector3 waypoint in _currentWaypoints)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawWireSphere(waypoint, 0.5f);
                }
        }
        void OnWaypointsValueChanged() => _currentWaypoints = _waypointsBBV.Value;
        void OnTargetPositionValueChanged() => _currentTargetPosition = _targetPositionBBV.Value;
        void OnTargetGameObjectValueChanged() => _currentTargetGameObject = _targetGameObjectBBV.Value;
    }
}
