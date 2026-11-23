using UnityEngine;
using UnityEngine.AI;

namespace NavMeshLesson
{
    /// <summary>
    /// Головне завдання «Втеча з лабіринту».
    /// Рівень 1: ворог один раз отримує ціль на старті й іде до нерухомого гравця.
    /// Рівень 2: SetDestination(target.position) всередині Update() — ворог переслідує гравця в реальному часі.
    /// updateEveryFrame перемикає між цими варіантами (у демці — клавіша [M]).
    /// </summary>
    public class EnemyChaser : MonoBehaviour, IRestartable
    {
        public Transform target;
        public bool updateEveryFrame = true;

        private NavMeshAgent agent;

        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            agent.SetDestination(target.position); // рівень 1: одна команда на старті
        }

        void Update()
        {
            if (updateEveryFrame)
            {
                agent.SetDestination(target.position); // рівень 2: оновлюємо ціль щокадру
            }
        }

        public void Restart()
        {
            if (agent == null) agent = GetComponent<NavMeshAgent>();
            if (agent.isOnNavMesh) agent.SetDestination(target.position);
        }

        public NavMeshAgent Agent => agent;
    }
}
