using UnityEngine;
using UnityEngine.AI; // Обов'язково підключаємо ШІ!

namespace NavMeshLesson
{
    /// <summary>
    /// Блок «Вчимо об'єкт ходити: C# та Agent» — скрипт з уроку.
    /// Вішається на капсулу з NavMeshAgent. На старті відправляє агента до цілі.
    /// Доповнення демо-сцени: Restart(), щоб демку можна було перезапустити.
    /// </summary>
    public class NPCMover : MonoBehaviour, IRestartable, IStatusSource
    {
        public Transform target;
        private NavMeshAgent agent;

        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            agent.SetDestination(target.position); // Відправляємо до цілі
        }

        public void Restart()
        {
            if (agent == null) agent = GetComponent<NavMeshAgent>();
            if (agent.isOnNavMesh) agent.SetDestination(target.position);
        }

        public string Status
        {
            get
            {
                if (agent == null || !agent.isOnNavMesh) return name + ": —";
                if (agent.pathPending) return name + ": рахує шлях…";
                return agent.remainingDistance <= agent.stoppingDistance + 0.05f
                    ? $"{name}: прибув (stoppingDistance {agent.stoppingDistance:F1} м)"
                    : $"{name}: іде, лишилось {agent.remainingDistance:F1} м, швидкість {agent.velocity.magnitude:F1} м/с";
            }
        }
    }
}
