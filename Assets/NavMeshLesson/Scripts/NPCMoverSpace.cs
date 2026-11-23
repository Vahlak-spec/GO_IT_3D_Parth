using UnityEngine;
using UnityEngine.AI;

namespace NavMeshLesson
{
    /// <summary>
    /// Міні-практика «Налаштування поведінки ШІ», рівень 2:
    /// NPC починає рух до цілі не зі старту гри, а тільки після натискання пробілу.
    /// </summary>
    public class NPCMoverSpace : MonoBehaviour, IRestartable, IStatusSource
    {
        public Transform target;
        private NavMeshAgent agent;

        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                agent.SetDestination(target.position);
            }
        }

        public void Restart()
        {
            if (agent == null) agent = GetComponent<NavMeshAgent>();
            if (agent.isOnNavMesh) agent.ResetPath(); // знову чекаємо пробіл
        }

        public string Status =>
            agent == null || !agent.hasPath ? "Чекаю на пробіл…" : $"Іду, лишилось {agent.remainingDistance:F1} м";
    }
}
