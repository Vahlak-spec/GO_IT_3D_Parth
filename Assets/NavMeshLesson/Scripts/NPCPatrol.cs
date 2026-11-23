using UnityEngine;
using UnityEngine.AI;

namespace NavMeshLesson
{
    /// <summary>
    /// Міні-практика «Налаштування поведінки ШІ», рівень 3:
    /// NPC іде до Target 1, а дійшовши (перевірка дистанції), розвертається й іде до Target 2.
    /// loop = true — патрулює між ними безкінечно.
    /// </summary>
    public class NPCPatrol : MonoBehaviour, IRestartable, IStatusSource
    {
        public Transform target1;
        public Transform target2;
        public float reachDistance = 0.6f;
        public bool loop = true;

        private NavMeshAgent agent;
        private Transform current;
        private bool finished;

        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            GoTo(target1);
        }

        void Update()
        {
            if (current == null || finished) return;

            // Порівнюємо дистанцію лише по горизонталі: ціль стоїть на підлозі (y = 0),
            // а центр капсули — на висоті ~1 м, тож звичайний Vector3.Distance ніколи не стане меншим за 1.
            Vector3 a = transform.position; a.y = 0f;
            Vector3 b = current.position;   b.y = 0f;

            if (Vector3.Distance(a, b) < reachDistance)
            {
                if (current == target1) GoTo(target2);
                else if (loop) GoTo(target1);
                else finished = true;
            }
        }

        void GoTo(Transform t)
        {
            current = t;
            agent.SetDestination(t.position);
        }

        public void Restart()
        {
            if (agent == null) agent = GetComponent<NavMeshAgent>();
            finished = false;
            if (agent.isOnNavMesh) GoTo(target1);
        }

        public string Status => current == null ? "—" : "Іду до: " + current.name;
    }
}
