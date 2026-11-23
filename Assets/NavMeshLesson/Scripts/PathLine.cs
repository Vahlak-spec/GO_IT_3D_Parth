using UnityEngine;
using UnityEngine.AI;

namespace NavMeshLesson
{
    /// <summary>
    /// Допоміжний скрипт демо: малює поточний шлях агента (agent.path.corners) через LineRenderer.
    /// Сам NavMesh видно лише в Scene View, а ця лінія видна і в Game View.
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    public class PathLine : MonoBehaviour
    {
        public NavMeshAgent agent;
        public float heightOffset = 0.15f;

        private LineRenderer line;

        void Awake()
        {
            line = GetComponent<LineRenderer>();
            line.useWorldSpace = true;
            line.positionCount = 0;
        }

        void LateUpdate()
        {
            if (agent == null || !agent.isActiveAndEnabled || !agent.hasPath)
            {
                line.positionCount = 0;
                return;
            }

            Vector3[] corners = agent.path.corners;
            line.positionCount = corners.Length;
            for (int i = 0; i < corners.Length; i++)
                line.SetPosition(i, corners[i] + Vector3.up * heightOffset);
        }
    }
}
